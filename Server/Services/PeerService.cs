using CentralStation.Client.Models;
using CentralStation.Data;
using CentralStation.Data.Models;
using CentralStation.Server.MappingExtensions;
using CoreLibrary.ConstantValues;
using CoreLibrary.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CentralStation.Server.Services;

internal class PeerService : IPeerService
{
    private readonly MainDbContext _mainDb;
    private readonly IHubContext<MainHub> _hubContext;
    private readonly TimeSpan _connectionLifeTime = new(0, 15, 0, 0); // 15 hours

    public PeerService(MainDbContext mainDb, IHubContext<MainHub> hubContext)
    {
        _mainDb = mainDb;
        _hubContext = hubContext;
    }

    public IAsyncEnumerable<PeerNodeRow> GetNodes(string peerType)
    {
        IQueryable<PeerNode> GetQueryBase()
        {
            return _mainDb.PeerNodes
               .Include(pn => pn.Peer)
               .Include(pn => pn.PeerConnections)
               .AsNoTracking();
        }

        IQueryable<PeerNode>? BuildQuery()
        {
            if (peerType.Equals(CoreConstants.BackEndPeer, StringComparison.InvariantCultureIgnoreCase))
            {
                return GetQueryBase().Where(pn => pn.Peer.Name != CoreConstants.FrontEndPeer);
            }
            else if (peerType.Equals(CoreConstants.FrontEndPeer, StringComparison.InvariantCultureIgnoreCase))
            {
                return GetQueryBase().Where(pn => pn.Peer.Name == CoreConstants.FrontEndPeer);
            }
            else
            {
                return null;
            }
        }

        var query = BuildQuery();
        if (query == null)
        {
            return new List<PeerNodeRow>().ConvertToAsyncEnum(); // fallback
        }

        return query.OrderBy(pn => pn.Peer.Name)
            .ThenByDescending(pn => pn.PeerConnections.OrderByDescending(pc => pc.LastMessageTime).Select(pc => pc.LastMessageTime).FirstOrDefault())
            .Select(pn => pn.MapToPeerNodeRow())
            .AsAsyncEnumerable();
    }

    /// <summary>
    /// Get leading PeerNode Id and if for some reason it wasn't found, appoint oldest node as leading node.
    /// </summary>
    public async Task<Guid> EnsureLeadingNodeId(Guid peerId)
    {
        var leadingNodeInfo = await _mainDb.PeerNodes
            .AsNoTracking()
            .Include(pn => pn.PeerConnections)
            .Where(pn => pn.PeerId == peerId && pn.IsLeading)
            .Select(pn => new { pn.Id, HasConnections = pn.PeerConnections.Any() })
            .FirstOrDefaultAsync();

        if (leadingNodeInfo != null)
        {
            if (!leadingNodeInfo.HasConnections)
            {
                throw new InvalidOperationException($"Leading {nameof(PeerNode)} has no connections.");
            }
            return leadingNodeInfo.Id;
        }

        // Find oldest node, that has at least one connection

        PeerNode? oldestNode = await _mainDb.PeerNodes
            .Include(pn => pn.PeerConnections)
            .Where(pn => pn.PeerId == peerId && pn.PeerConnections.Any())
            .OrderBy(pn => pn.CreationTime)
            .FirstOrDefaultAsync();

        if (oldestNode == null)
        {
            throw new KeyNotFoundException($"Unable to find {nameof(PeerNode)} with a connection.");
        }

        oldestNode.IsLeading = true;
        await _mainDb.SaveChangesAsync();
        return oldestNode.Id;
    }

    public IAsyncEnumerable<PeerConnectionRow> GetConnections(string peerType)
    {
        IQueryable<PeerConnection> GetQueryBase()
        {
            return _mainDb.PeerConnections
                .Include(pc => pc.PeerNode).ThenInclude(pn => pn == null ? null : pn.Peer)
                .AsNoTracking();
        }

        IQueryable<PeerConnection>? BuildQuery()
        {
            if (peerType.Equals(CoreConstants.BackEndPeer, StringComparison.InvariantCultureIgnoreCase))
            {
                return GetQueryBase().Where(p => p.PeerNode == null || p.PeerNode.Peer.Name != CoreConstants.FrontEndPeer);
            }
            else if (peerType.Equals(CoreConstants.FrontEndPeer, StringComparison.InvariantCultureIgnoreCase))
            {
                return GetQueryBase().Where(p => p.PeerNode == null || p.PeerNode.Peer.Name == CoreConstants.FrontEndPeer);
            }
            else
            {
                return null;
            }
        }

        var query = BuildQuery();
        if (query == null)
        {
            return new List<PeerConnectionRow>().ConvertToAsyncEnum(); // fallback
        }

        return query.OrderBy(pc => pc.PeerNode == null ? String.Empty : pc.PeerNode.Peer.Name)
            .ThenByDescending(pc => pc.LastMessageTime)
            .Select(pc => pc.MapToPeerConnectionRow())
            .AsAsyncEnumerable();
    }

    /// <summary>
    /// Deletes all expired connections.
    /// Then Deletes the nodes that are left with zero connections.
    /// Connections that are removed should be able to be re-added when it pushes a message (for instance SingOflife).
    /// Tested and works for individual deletions frontend connections (back and front).
    /// TODO: to be tested:
    /// - what if PurgeExpiredConnections does it? (first make it occur more often (there's another todo for that)
    /// - also test SignOfLife specifically, for frontend and backend connections.
    /// </summary>
    public async Task DeleteExpired(CancellationToken? cancellation = null)
    {
        UpdatedRegistrations? toUpdate = null;
        
        // TODO: use raw SQL for this...
        DateTime connectionExpiry = DateTime.UtcNow.Add(-_connectionLifeTime);
        var connections = _mainDb.PeerConnections.Where(pc => pc.PeerNodeId.HasValue && pc.LastMessageTime < connectionExpiry);

        foreach (PeerConnection connection in connections)
        {
            await UnregisterConnectionInHub(connection);
            _mainDb.Remove(connection);
            toUpdate = UpdatedRegistrations.Connection;
        }

        if (toUpdate != null)
        {
            await _mainDb.SaveChangesAsync();
        }

        if (await DeleteEmptyNodes(true))
        {
            toUpdate = UpdatedRegistrations.PeerNode;
        }

        if (!cancellation.HasValue || !cancellation.Value.IsCancellationRequested)
        {
            await _hubContext.Clients.Group(CoreConstants.FrontEndPeer).SendAsync(SignalrMessages.RegistrationUpdate, toUpdate);
        }
    }

    /// <summary>
    /// Deletes all connections, except given connectinIds.
    /// Then Deletes the nodes (back and front) that are left with zero connections.
    /// </summary>
    public async Task DeleteAllExcept(List<string> excludedConnections)
    {
        UpdatedRegistrations? toUpdate = null;

        // TODO: use raw SQL for this...
        var connections = _mainDb.PeerConnections.Where(pc => !excludedConnections.Contains(pc.ConnectionId));

        foreach (PeerConnection connection in connections)
        {
            await UnregisterConnectionInHub(connection);
            _mainDb.Remove(connection);
            toUpdate = UpdatedRegistrations.Connection;
        }

        if (toUpdate != null)
        {
            await _mainDb.SaveChangesAsync();
        }

        if (await DeleteEmptyNodes(false))
        {
            toUpdate = UpdatedRegistrations.PeerNode;
        }

        await _hubContext.Clients.Group(CoreConstants.FrontEndPeer).SendAsync(SignalrMessages.RegistrationUpdate, toUpdate);
    }

    /// <summary>
    /// Remove FrontEnd PeerNodes without PeerConnections. Back-end PeerNodes without PeerConnections are allowed.
    /// </summary>
    private async Task<bool> DeleteEmptyNodes(bool excludeBackendNodes)
    {
        bool removePending = false;

        // TODO: use raw SQL for this...
        var nodes = _mainDb.PeerNodes
            .Include(pn => pn.Peer)
            .Where(pn => (excludeBackendNodes == false || pn.Peer.Name == CoreConstants.FrontEndPeer)
                         && pn.PeerConnections.Any() == false);

        foreach (PeerNode expiredNode in nodes)
        {
            _mainDb.Remove(expiredNode);
            removePending = true;
        }

        if (removePending)
        {
            await _mainDb.SaveChangesAsync();
        }

        return removePending;
    }
    
    public async Task<bool> DeletePeer(Guid id)
    {
        Peer? peer = await _mainDb.Peers.Include(p => p.PeerNodes).ThenInclude(pn => pn.PeerConnections).FirstOrDefaultAsync(p => p.Id == id);
        if (peer == null)
        {
            return false;
        }
        if (peer.Name == CoreConstants.FrontEndPeer)
        {
            throw new Exception("Deleting front-end peer not allowed."); // TODO: should be caught in general exception handler
        }

        var connections = peer.PeerNodes.SelectMany(pn => pn.PeerConnections);
        foreach (var connection in connections)
        {
            await UnregisterConnectionInHub(connection);
        }

        _mainDb.Remove(peer);
        await _mainDb.SaveChangesAsync();

        await _hubContext.Clients.Group(CoreConstants.FrontEndPeer).SendAsync(SignalrMessages.RegistrationUpdate, UpdatedRegistrations.Peer);
        return true;
    }

    public async Task<bool> DeleteNode(Guid id)
    {
        PeerNode? node = await _mainDb.PeerNodes.Include(pn => pn.PeerConnections).FirstOrDefaultAsync(p => p.Id == id);
        if (node == null)
        {
            return false;
        }

        foreach (var connection in node.PeerConnections)
        {
            await UnregisterConnectionInHub(connection);
            _mainDb.Remove(connection);
        }

        _mainDb.Remove(node);
        await _mainDb.SaveChangesAsync();

        await _hubContext.Clients.Group(CoreConstants.FrontEndPeer).SendAsync(SignalrMessages.RegistrationUpdate, UpdatedRegistrations.PeerNode);
        return true;
    }

    public async Task<bool> DeleteConnection(Guid id)
    {
        PeerConnection? connection = await _mainDb.PeerConnections.FirstOrDefaultAsync(p => p.Id == id);
        if (connection == null)
        {
            return false;
        }

        await UnregisterConnectionInHub(connection);

        _mainDb.Remove(connection);
        await _mainDb.SaveChangesAsync();

        await _hubContext.Clients.Group(CoreConstants.FrontEndPeer).SendAsync(SignalrMessages.RegistrationUpdate, UpdatedRegistrations.Connection);
        return true;
    }

    private async Task UnregisterConnectionInHub(PeerConnection connection)
    {
        await _hubContext.Groups.RemoveFromGroupAsync(connection.ConnectionId, CoreConstants.FrontEndPeer); // This should not yield an error even when it's not a front-end connection

        string? peerNodeId = connection.PeerNodeId?.ToString();
        if (!String.IsNullOrEmpty(peerNodeId))
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connection.ConnectionId, peerNodeId);
        }
    }
}
