using CentralStation.Client.Models;
using CentralStation.Data.Models;

namespace CentralStation.Server.MappingExtensions
{
    public static class PeerConnectionMappingExtensions
    {
        public static PeerConnectionRow MapToPeerConnectionRow(this PeerConnection value)
        {
            return new PeerConnectionRow
            {
                Id = value.Id,
                PeerName = value.PeerNode?.Peer.Name ?? String.Empty,
                HubConnectionId = value.ConnectionId,
                IP = value.IP,
                LastMessageTime = value.LastMessageTime
            };
        }
    }
}
