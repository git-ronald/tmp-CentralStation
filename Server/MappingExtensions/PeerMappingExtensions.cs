using CentralStation.Client.Models;
using CentralStation.Data.Models;

namespace CentralStation.Server.MappingExtensions
{
    public static class PeerMappingExtensions
    {
        public static PeerRow MapToPeerRow(this Peer value)
        {
            return new PeerRow
            {
                Id = value.Id,
                Name = value.Name,
                PeerNodeCount = value.PeerNodes.Count
            };
        }
    }
}
