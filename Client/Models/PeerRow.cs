namespace CentralStation.Client.Models
{
    public class PeerRow
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = String.Empty;
        public int PeerNodeCount { get; set; } = 0;
        public bool MarkDelete { get; set; } = false;
    }
}
