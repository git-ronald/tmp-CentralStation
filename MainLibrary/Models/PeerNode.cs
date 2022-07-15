using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CentralStation.Data.Models
{
    [Index(nameof(CreationTime))]
    [Index(nameof(Name))]
    public class PeerNode
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PeerId { get; set; }
        public Peer Peer { get; set; } = new();
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(50)]
        public string LastIP { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public int SignOfLifeEvent { get; set; }
        public bool IsLeading { get; set; } = false;
        public ICollection<PeerConnection> PeerConnections { get; set; } = new List<PeerConnection>();
    }
}
