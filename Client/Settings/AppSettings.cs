namespace CentralStation.Client.Settings
{
    public class AppSettings
    {
        public string[] AllowedScopes { get; set; } = Array.Empty<string>();
        public string AppTitle { get; set; } = String.Empty;
    }
}
