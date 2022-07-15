using CentralStation.Client.Models;
using CentralStation.Data.Models;
using Microsoft.AspNetCore.Components.Routing;

namespace CentralStation.Server.MappingExtensions;

public static class NavPageMappingExtensions
{
    public static ClientNavPage MapToClientNavPage(this NavPage value)
    {
        var result = new ClientNavPage
        {
            Id = value.Id,
            ParentId = value.ParentId,
            Children = value.Children.Select(c => c.MapToClientNavPage()).ToList(),
            Url = value.Url,
            Label = value.Label,
            Icon = value.Icon,
            Match = (NavLinkMatch)value.NavLinkMatch,
            IsInNavMenu = value.IsInNavMenu,
            Order = value.Order
        };
        return result;
    }
}
