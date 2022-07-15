using CentralStation.Client.Shared.Dialog;
using CoreLibrary.Delegates;
using CoreLibrary.Helpers;

namespace CentralStation.Client.Services
{
    public class CentralManagementService : ICentralManagementService
    {
        public CentralManagementService(INavigationService navigationService, IHubClientService hubClientService)
        {
            Navigation = navigationService;
            HubClient = hubClientService;
        }

        public event EmptyAsyncHandler? NotifyClearState;

        public INavigationService Navigation { get; }
        public IHubClientService HubClient { get; }

        public Func<string, IBuilderReady> BuildDialog { get; set; } = _ => ModalDialog.BuildEmpty();
        public Func<Task> CloseDialog { get; set; } = () => Task.CompletedTask;

        public Task InvokeNotifyClearState() => NotifyClearState.InvokeHandlers();

        public async Task HandleAuhtorized()
        {
            await NotifyClearState.InvokeHandlers();
            Navigation.IsSecured = true;

            var task1 = HubClient.StartConnection();
            var task2 = Navigation.FetchNavPages();
        }

        public async Task HandleNotAuthorized()
        {
            await NotifyClearState.InvokeHandlers();
            Navigation.IsSecured = false;

            var task1 = HubClient.StopConnection();
            var task2 = Navigation.FetchNavPages();
        }
    }
}
