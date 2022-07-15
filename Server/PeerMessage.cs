namespace CentralStation.Server
{
    public class PeerMessage
    {
        public string PeerNodeId { get; set; } = String.Empty;
        public string Path { get; set; } = String.Empty;
        public object? Data { get; set; }
    }
}
