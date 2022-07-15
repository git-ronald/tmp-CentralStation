using Microsoft.AspNetCore.Components.Routing;

namespace CentralStation.Client.Models
{
    public class ClientNavPage
    {
        public ClientNavPage() { }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? ParentId { get; set; }
        public ICollection<ClientNavPage> Children { get; set; } = new List<ClientNavPage>();
        public string Url { get; set; } = String.Empty;
        public string Label { get; set; } = String.Empty;
        public string Icon { get; set; } = String.Empty;
        public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
        public bool IsInNavMenu { get; set; }
        public int Order { get; set; } = 0;
    }
}
