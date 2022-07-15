namespace CentralStation.Client.Models
{
    public class PeerNodeRow
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PeerName { get; set; } = string.Empty;
        public string Name { get; set; } = String.Empty;
        public bool IsLeading { get; set; } = false;
        public string[] HubConnectionIds { get; set; } = Array.Empty<string>();
        public DateTime? LastMessageTime { get; set; }
        public bool MarkDelete { get; set; } = false;
    }
}
