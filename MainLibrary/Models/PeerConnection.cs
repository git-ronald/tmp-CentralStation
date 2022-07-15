using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CentralStation.Data.Models
{
    [Index(nameof(ConnectionId))]
    [Index(nameof(LastMessageTime))]
    public class PeerConnection
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? PeerNodeId { get; set; } = null;
        public PeerNode? PeerNode { get; set; } = null;
        public string ConnectionId { get; set; } = string.Empty;
        [MaxLength(50)]
        public string IP { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; } = DateTime.UtcNow;
    }
}
