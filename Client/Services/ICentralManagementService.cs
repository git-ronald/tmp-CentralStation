using CentralStation.Client.Shared.Dialog;
using CoreLibrary.Delegates;

namespace CentralStation.Client.Services
{
    public interface ICentralManagementService
    {
        IHubClientService HubClient { get; }
        INavigationService Navigation { get; }
        Func<string, IBuilderReady> BuildDialog { get; set; }
        Func<Task> CloseDialog { get; set; }

        event EmptyAsyncHandler? NotifyClearState;

        Task HandleAuhtorized();
        Task HandleNotAuthorized();
        Task InvokeNotifyClearState();
    }
}