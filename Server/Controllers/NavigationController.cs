using CentralStation.Client.Models;
using CentralStation.Data;
using CentralStation.Data.Models;
using CentralStation.Server.BusinessExtensions;
using CentralStation.Server.MappingExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace CentralStation.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2c:Scopes")]
public class NavigationController : ControllerBase
{
    private readonly MainDbContext _mainDb;

    public NavigationController(MainDbContext mainDb)
    {
        _mainDb = mainDb;
    }

    [HttpGet("pages")]
    public IEnumerable<ClientNavPage> GetPages()
    {
        // TODO: ensure that first of generation of pages never has children
        var result = GetTestNavPages().Select(p => p.MapToClientNavPage());
        return result;
    }

    public IEnumerable<NavPage> GetTestNavPages()
    {
        yield return new NavPage { Label = "Testing", Icon = "oi-list-rich", Url = "test1"};
        yield return GetTestTree();
    }

    private NavPage GetTestTree()
    {
        NavPage granddaddy = new() { Label = "Granddaddy", Icon = "oi-plus", Url = "granddaddy" };
        NavPage mommy = new() { Label = "Mommy", Icon = "oi-plus", Url = "granddaddy/mommy" };
        NavPage daddy = new() { Label = "Daddy", Icon = "oi-list-rich", Url = "granddaddy/daddy" };
        NavPage uncle = new() { Label = "Uncle", Icon = "oi-home", Url = "granddaddy/uncle" };
        NavPage baby = new() { Label = "Baby", Icon = "oi-plus", Url = "granddaddy/mommy/baby" };

        mommy.AddChildren(baby);
        return granddaddy.AddChildren(daddy, mommy, uncle);
    }
}
