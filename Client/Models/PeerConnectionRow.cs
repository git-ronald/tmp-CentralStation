namespace CentralStation.Client.Models
{
    public class PeerConnectionRow
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PeerName { get; set; } = String.Empty;
        public string HubConnectionId { get; set; } = String.Empty;
        public string IP { get; set; } = String.Empty;
        public DateTime LastMessageTime { get; set; } = DateTime.UtcNow;
        public bool MarkDelete { get; set; } = false;
    }
}
