using CentralStation.Data;
using CentralStation.Data.Models;
using CentralStation.Server.BusinessExtensions;
using CentralStation.Server.ConstantValues;
using CentralStation.Server.Services;
using CoreLibrary.ConstantValues;
using CoreLibrary.Helpers;
using CoreLibrary.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CentralStation.Server
{
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{AuthenticationConstants.PeerScheme}")]
    internal class MainHub : Hub
    {
        private readonly MainDbContext _mainDb;
        private readonly IPeerService _peerService;

        private Queue<PeerMessage> _pendingMessages = new();

        public MainHub(MainDbContext mainDb, IPeerService peerService)
        {
            _mainDb = mainDb;
            _peerService = peerService;
        }

        public async Task TestRequest()
        {
            await UpdateConnctionRegistration();
            await Clients.Client(Context.ConnectionId).SendAsync(SignalrMessages.TestResponse);
        }

        public Task NotifySignOfLife() => UpdateConnctionRegistration();

        public async Task RequestPeer(Guid peerId, string path, object? requestData)
        {
            bool awaitRegistration = await UpdateConnctionRegistration(); // Registration of frontend
            Guid leadingNodeId = await _peerService.EnsureLeadingNodeId(peerId);

            PeerMessage message = new()
            {
                PeerNodeId = leadingNodeId.ToString(),
                Path = path,
                Data = requestData
            };

            if (awaitRegistration)
            {
                _pendingMessages.Enqueue(message);
            }
            else
            {
                await Clients.Group(message.PeerNodeId).SendAsync(SignalrMessages.PeerRequest, message.Path, message.Data);
            }
        }

        public async Task PeerResponse(string path, object? responseData)
        {
            var peerConnectionId = await GetPeerConnectionId();
            if (peerConnectionId != Guid.Empty)
            {
                await Clients.Group(CoreConstants.FrontEndPeer).SendAsync(nameof(PeerResponse), peerConnectionId, path, responseData);
            }
        }

        public async Task PeerError(string path, object requestData, HttpStatusCode statusCode)
        {
            var peerConnectionId = await GetPeerConnectionId();
            if (peerConnectionId != Guid.Empty)
            {
                await Clients.Group(CoreConstants.FrontEndPeer).SendAsync(nameof(PeerError), peerConnectionId, path, requestData, statusCode);
            }
        }

        private Task<Guid> GetPeerConnectionId()
        {
            return _mainDb.PeerConnections
                .AsNoTracking()
                .Where(pc => pc.ConnectionId == Context.ConnectionId)
                .Select(pc => pc.Id)
                .FirstOrDefaultAsync();
        }

        private async Task<bool> UpdateConnctionRegistration()
        {
            if (!Context.TryGetCallerIP(out string ip))
            {
                throw new InvalidOperationException("Unable to obtain caller IP.");
            }

            (Guid peerConnectionId, bool awaitRegistration) = await EnsureConnectionRegistration(ip);

            PeerConnection? connection = _mainDb.PeerConnections.Include(pc => pc.PeerNode).FirstOrDefault(pc => pc.Id == peerConnectionId);
            if (connection == null)
            {
                throw new KeyNotFoundException($"Unable to find {nameof(PeerConnection)}."); // should never happen
            }

            connection.IP = ip;
            connection.LastMessageTime = DateTime.UtcNow;
            (connection.PeerNode ?? new()).LastIP = ip;
            await _mainDb.SaveChangesAsync();

            await Clients.Group(CoreConstants.FrontEndPeer).SendAsync(SignalrMessages.RegistrationUpdate, UpdatedRegistrations.Connection);
            return awaitRegistration;
        }

        public async Task PeerRegistrationInfoResponse(PeerRegistrationInfo info)
        {
            if (!Context.TryGetCallerIP(out string ip))
            {
                return;
            }

            // PeerConnection was added in HandleIncomingBackEndConnection, but without a PeerNode
            var peerConnection = await _mainDb.PeerConnections
                .FirstOrDefaultAsync(pc => pc.ConnectionId == Context.ConnectionId
                                           && !pc.PeerNodeId.HasValue
                                           && pc.IP == ip);
            if (peerConnection == null)
            {
                return;
            }

            UpdatedRegistrations? updated = null;

            async Task<(Peer Peer, bool IsNew)> GetPeer()
            {
                if (info.PeerName == CoreConstants.FrontEndPeer)
                {
                    Peer? peer = await _mainDb.Peers.FirstOrDefaultAsync(p => p.Name == CoreConstants.FrontEndPeer);
                    if (peer != null)
                    {
                        return (peer, false);
                    }
                    return (new Peer { Name = CoreConstants.FrontEndPeer }, true);
                }
                else
                {
                    Peer? peer = await _mainDb.Peers.FirstOrDefaultAsync(p => p.Id == info.PeerId);
                    if (peer != null)
                    {
                        return (peer, false);
                    }
                    return (new Peer { Id = info.PeerId, Name = info.PeerName }, true);
                }
            }

            (Peer peer, bool isNewPeer) = await GetPeer();
            if (isNewPeer)
            {
                await _mainDb.AddAsync(peer);
                updated ??= UpdatedRegistrations.Peer;
            }

            Task<List<PeerNode>> GetPeerNodeFamily()
            {
                if (info.PeerName == CoreConstants.FrontEndPeer)
                {
                    return _mainDb.PeerNodes.Where(pn => pn.Id == info.PeerNodeId).ToListAsync();
                }
                return _mainDb.PeerNodes.Where(pn => pn.PeerId == peer.Id).ToListAsync();
            }
            var peerNodeFamily = await GetPeerNodeFamily();

            var currentNode = peerNodeFamily.FirstOrDefault(pn => pn.Id == info.PeerNodeId);
            if (currentNode == null)
            {
                bool isLeading = (info.PeerName != CoreConstants.FrontEndPeer && !peerNodeFamily.Any(pn => pn.IsLeading));

                currentNode = new PeerNode
                {
                    Id = info.PeerNodeId,
                    Peer = peer,
                    Name = $"{info.PeerName} - {ip}",
                    LastIP = ip,
                    SignOfLifeEvent = (short)info.ConfirmedSignOfLifeEvent.TotalMinutes,
                    IsLeading = isLeading
                };

                await _mainDb.AddAsync(currentNode);
                updated ??= UpdatedRegistrations.PeerNode;
            }
            else
            {
                if (ip != currentNode.LastIP)
                {
                    currentNode.LastIP = ip;
                    updated ??= UpdatedRegistrations.PeerNode;
                }
            }

            peerConnection.PeerNode = currentNode;
            peerConnection.LastMessageTime = DateTime.UtcNow;
            await _mainDb.SaveChangesAsync();

            updated ??= UpdatedRegistrations.Connection;

            await Groups.AddToGroupAsync(Context.ConnectionId, currentNode.Id.ToString());
            await Clients.Clients(Context.ConnectionId).SendAsync(SignalrMessages.PeerRegistrationConfirmed);

            if (info.PeerName == CoreConstants.FrontEndPeer)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, CoreConstants.FrontEndPeer);

                while (_pendingMessages.Count > 0)
                {
                    var message = _pendingMessages.Dequeue();
                    await Clients.Group(message.PeerNodeId).SendAsync(message.Path, message.Data);
                }
            }

            await Clients.Group(CoreConstants.FrontEndPeer).SendAsync(SignalrMessages.RegistrationUpdate, updated);
        }

        public override async Task OnConnectedAsync()
        {
            if (!Context.TryGetCallerIP(out string ip))
            {
                await base.OnConnectedAsync();
                return;
            }

            await EnsureConnectionRegistration(ip);
            await base.OnConnectedAsync();
        }

        private async Task<(Guid Id, bool AwaitRegistration)> EnsureConnectionRegistration(string ip)
        {
            PeerConnection? existingConnection = await _mainDb.PeerConnections.AsNoTracking().FirstOrDefaultAsync(pc => pc.ConnectionId == Context.ConnectionId);
            if (existingConnection != null)
            {
                bool awaitPeerNodeRegistration = await EnsurePeerNodeRegistration(existingConnection.PeerNodeId);
                return (existingConnection.Id, awaitPeerNodeRegistration);
            }

            // Add new PeerConnection without PeerNode. The PeerNode will be supplied in PeerRegistrationInfoResponse
            PeerConnection newConnection = new() { ConnectionId = Context.ConnectionId, IP = ip };
            await _mainDb.AddAsync(newConnection);
            await _mainDb.SaveChangesAsync();

            // TODO: also make scheduled task to rearrange SignOfLifeEvent according to most optimal distribution based on PeerNode.SignOfLifeEvent at that moment.

            await Clients.Clients(Context.ConnectionId).SendAsync(SignalrMessages.RequestPeerRegistrationInfo, CalcSignOfLifeEvent());
            return (newConnection.Id, true);
        }

        private async Task<bool> EnsurePeerNodeRegistration(Guid? peerNodeId)
        {
            PeerNode? peerNode = peerNodeId.HasValue ? await _mainDb.PeerNodes.FirstOrDefaultAsync(pn => pn.Id == peerNodeId) : null;
            if (peerNode == null)
            {
                await Clients.Clients(Context.ConnectionId).SendAsync(SignalrMessages.RequestPeerRegistrationInfo, CalcSignOfLifeEvent());
                return true;
            }
            return false;
        }

        // TODO: SignOfLife should occur more often (every 15 minutes?),
        // therefore FindEmptiestPositionInTimeFrame should be calculated in seconds instead of minutes.
        // Perhaps the accompanying table field should also change.
        private TimeSpan CalcSignOfLifeEvent()
        {
            var today = DateTime.Today; // Local time instead of UTC. We simply need a time-frame of 00:00AM - 12:00PM. TZ and even date are irrelevant.

            TimeSpan optimalTime = _mainDb.PeerNodes
                .AsNoTracking()
                .Where(pn => pn.SignOfLifeEvent > 0)
                .Select(pn => TimeSpan.FromMinutes(pn.SignOfLifeEvent))
                .AsEnumerable()
                .FindEmptiestPositionInTimeFrame(today, today.AddHours(6))
                .TimeOfDay;

            return new TimeSpan(optimalTime.Hours, optimalTime.Minutes, 0);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var peerConnection = await _mainDb.PeerConnections.FirstOrDefaultAsync(pc => pc.ConnectionId == Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, CoreConstants.FrontEndPeer); // This should not yield an error even when it's not a front-end connection

            string? peerNodeId = peerConnection?.PeerNodeId?.ToString();
            if (!String.IsNullOrEmpty(peerNodeId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, peerNodeId);
            }

            if (peerConnection != null)
            {
                _mainDb.Remove(peerConnection);
                await _mainDb.SaveChangesAsync();

                await Clients.Group(CoreConstants.FrontEndPeer).SendAsync(SignalrMessages.RegistrationUpdate, UpdatedRegistrations.Connection);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
