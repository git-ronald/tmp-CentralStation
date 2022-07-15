using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CentralStation.Data.Models
{
    [Index(nameof(IsInNavMenu))]
    [Index(nameof(Order))]
    public class NavPage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? ParentId { get; set; } = null;
        public Guid? PeerNodeId { get; set; } = null;
        public PeerNode? PeerNode { get; set; } = null;
        public NavPage? Parent { get; set; } = null;
        public ICollection<NavPage> Children { get; set; } = new List<NavPage>();
        public string Url { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int NavLinkMatch { get; set; } = 0; // 0 == Prefix
        public bool IsInNavMenu { get; set; } = true;
        public int Order { get; set; } = 0;
    }
}
