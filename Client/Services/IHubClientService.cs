using CoreLibrary.Delegates;

namespace CentralStation.Client.Services
{
    public interface IHubClientService
    {
        event EmptyAsyncHandler? OnConnectionRegistered;
        event HubClientService.RegistrationUpdateDelegate? OnRegistrationUpdate;
        event HubClientService.PeerResponseDelegate? OnPeerResponse;
        event HubClientService.PeerErorDelegate? OnPeerEror;

        bool IsConnected { get; }
        string? HubConnectionId { get; }

        Task StartConnection();
        Task StopConnection();
        Task RequestPeer(Guid peerId, string path, object? data);
    }
}
