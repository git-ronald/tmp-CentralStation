using CentralStation.Client.Models;

namespace CentralStation.Server.Services
{
    public interface IPeerService
    {
        Task DeleteAllExcept(List<string> excludedIds);
        Task<bool> DeleteConnection(Guid id);
        Task<bool> DeleteNode(Guid id);
        Task<bool> DeletePeer(Guid id);
        IAsyncEnumerable<PeerConnectionRow> GetConnections(string peerType);
        IAsyncEnumerable<PeerNodeRow> GetNodes(string peerType);
        Task DeleteExpired(CancellationToken? cancellation = null);
        Task<Guid> EnsureLeadingNodeId(Guid peerId);
    }
}