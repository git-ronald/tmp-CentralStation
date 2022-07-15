using CentralStation.Client.Extensions;
using CentralStation.Client.Models;
using CentralStation.Client.Settings;
using CoreLibrary.Delegates;
using CoreLibrary.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace CentralStation.Client.Services;

public class NavigationService : INavigationService
{
    private readonly AppSettings _appSettings;
    private readonly HttpClient _http;
    private readonly NavigationManager _navigationManager;

    private Guid? _parentPageId = null;
    private Dictionary<Guid, ClientNavPage> _pages = new();
    private Dictionary<string, ClientNavPage> _urlMapping = new();
    private List<ClientNavPage> _rootPages = new();

    public NavigationService(IOptions<AppSettings> options, HttpClient http, NavigationManager navigationManager)
    {
        _appSettings = options.Value;
        _http = http;

        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    public event EmptyAsyncHandler? NotifyMenuUpdate;

    public bool IsSecured { get; set; }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        var currentParentId = GetCurrentPage()?.ParentId;
        if (currentParentId == _parentPageId)
        {
            return;
        }
        _parentPageId = currentParentId;
        NotifyMenuUpdate.InvokeHandlers();
    }

    public string GetPageTitle()
    {
        List<string> result = new() { _appSettings.AppTitle };

        void AddPageLabel()
        {
            var parentId = GetCurrentPage()?.ParentId;
            if (!parentId.HasValue)
            {
                return;
            }
            if (!_pages.TryGetValue(parentId.Value, out ClientNavPage? page))
            {
                return;
            }

            result.Add(page.Label);
        }

        AddPageLabel();
        return String.Join(" | ", result);
    }

    public async Task FetchNavPages()
    {
        async Task<IEnumerable<ClientNavPage>> Fetch()
        {
            if (!IsSecured)
            {
                return GetPublicPages();
            }

            var defaultSecuredPages = GetDefaultSecuredPages();
            var securedPages = await _http.GetFromJsonAsync<IEnumerable<ClientNavPage>>("navigation/pages") ?? Enumerable.Empty<ClientNavPage>();
            return defaultSecuredPages.Concat(securedPages).AutoOrder();
        }

        RegisterPages(await Fetch());
        _parentPageId = GetCurrentPage()?.ParentId;
        await NotifyMenuUpdate.InvokeHandlers();
    }

    private IEnumerable<ClientNavPage> GetPublicPages()
    {
        yield return new ClientNavPage { Url = GetBreadcrumbRoot().Url, Label = "Start", Icon = "oi-home", IsInNavMenu = true, Match = NavLinkMatch.All };
    }

    private IEnumerable<ClientNavPage> GetDefaultSecuredPages()
    {
        yield return new ClientNavPage { Url = GetBreadcrumbRoot().Url, Label = "Dashboard", Icon = "oi-plus", IsInNavMenu = true, Match = NavLinkMatch.All };

        yield return new ClientNavPage { Label = "Settings", Icon = "oi-wrench", IsInNavMenu = false }.AddChildren(
            new ClientNavPage { Url = "settings/general", Label = "General", Icon = "oi-wrench", IsInNavMenu = true },
            new ClientNavPage { Url = "settings/hub/peers", Label = "Peers", Icon = "oi-plus", IsInNavMenu = true },
            new ClientNavPage { Url = "settings/hub/nodes/frontend", Label = "Frontend nodes", Icon = "oi-plus", IsInNavMenu = true },
            new ClientNavPage { Url = "settings/hub/nodes/backend", Label = "Backend nodes", Icon = "oi-plus", IsInNavMenu = true },
            new ClientNavPage { Url = "settings/hub/connections/frontend", Label = "Frontend connections", Icon = "oi-plus", IsInNavMenu = true },
            new ClientNavPage { Url = "settings/hub/connections/backend", Label = "Backend connections", Icon = "oi-plus", IsInNavMenu = true });
    }

    private void RegisterPages(IEnumerable<ClientNavPage> pages)
    {
        _pages = new Dictionary<Guid, ClientNavPage>();
        _urlMapping = new Dictionary<string, ClientNavPage>();
        _rootPages = new List<ClientNavPage>();

        void LoopPageTree(IEnumerable<ClientNavPage> pagesOfGeneration)
        {
            foreach (var page in pagesOfGeneration)
            {
                _pages[page.Id] = page;
                _urlMapping[page.Url] = page;

                if (!page.ParentId.HasValue)
                {
                    _rootPages.Add(page);
                }

                if (page.Children.Any())
                {
                    LoopPageTree(page.Children);
                    page.Url = page.Children.OrderBy(p => p.Order).First().Url;
                }
            }
        }

        LoopPageTree(pages);
    }

    private ClientNavPage? GetCurrentPage()
    {
        var url = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
        if (_urlMapping.TryGetValue(url, out var page))
        {
            return page;
        }
        return null;
    }

    public IEnumerable<ClientNavPage> GetMenuPages()
    {
        IEnumerable<ClientNavPage> GetPages()
        {
            if (!_parentPageId.HasValue)
            {
                return _rootPages;
            }

            return _pages[_parentPageId.Value].Children;
        }
        return GetPages().Where(p => p.IsInNavMenu).OrderBy(p => p.Order);
    }

    public List<BreadcrumbNavPage> GetBreadcrumb()
    {
        List<BreadcrumbNavPage> result = new();

        void BuildBreadcrumb(ClientNavPage? page)
        {
            if (page != null && page.ParentId.HasValue)
            {
                var parent = _pages[page.ParentId.Value];
                result.Add(new BreadcrumbNavPage { Label = parent.Label, Url = parent.Url });
                BuildBreadcrumb(_pages[page.ParentId.Value]);
            }
        }

        BuildBreadcrumb(GetCurrentPage());

        var rootPage = GetBreadcrumbRoot();
        if (result.Count > 1)
        {
            result[0].Url = result[1].Url;
        }
        else
        {
            result[0].Url = rootPage.Url;
        }

        result.Add(rootPage);
        return result;
    }

    public BreadcrumbNavPage GetBreadcrumbRoot()
    {
        BreadcrumbNavPage result = new() { Label = "Home" };
        result.Url = IsSecured ? "dashboard" : String.Empty;
        return result;
    }
}