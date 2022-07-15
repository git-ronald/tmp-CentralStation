using CentralStation.Client.Models;
using CoreLibrary.Delegates;

namespace CentralStation.Client.Services
{
    public interface INavigationService
    {
        bool IsSecured { get; set; }

        event EmptyAsyncHandler? NotifyMenuUpdate;

        Task FetchNavPages();
        List<BreadcrumbNavPage> GetBreadcrumb();
        IEnumerable<ClientNavPage> GetMenuPages();
        BreadcrumbNavPage GetBreadcrumbRoot();
        string GetPageTitle();
    }
}