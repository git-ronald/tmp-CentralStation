using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CentralStation.Data.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Peer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public ICollection<PeerNode> PeerNodes { get; set; } = new List<PeerNode>();
    }
}
