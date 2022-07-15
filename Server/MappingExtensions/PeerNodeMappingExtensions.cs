using CentralStation.Client.Models;
using CentralStation.Data.Models;

namespace CentralStation.Server.MappingExtensions
{
    public static class PeerNodeMappingExtensions
    {
        public static PeerNodeRow MapToPeerNodeRow(this PeerNode value)
        {
            DateTime? GetLastMessageTime()
            {
                var lastConnection = value.PeerConnections.OrderByDescending(pc => pc.LastMessageTime).FirstOrDefault();
                return lastConnection != null ? lastConnection.LastMessageTime : null;
            }

            return new PeerNodeRow
            {
                Id = value.Id,
                Name = value.Name,
                PeerName = value.Peer.Name,
                IsLeading = value.IsLeading,
                HubConnectionIds = value.PeerConnections.Select(pc => pc.ConnectionId).ToArray(),
                LastMessageTime = GetLastMessageTime()
            };
        }
    }
}
