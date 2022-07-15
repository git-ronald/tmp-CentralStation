namespace CentralStation.Client
{
    public class AccessTokenInfo
    {
        public string Value { get; set; } = String.Empty;
        public DateTime Expiration { get; set; } = DateTime.MinValue;
    }
}
