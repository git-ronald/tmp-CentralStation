using CoreLibrary.Helpers;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace CentralStation.Server.BusinessExtensions;

public static class HubExtensions
{
    public static bool TryGetCallerIP(this HubCallerContext context, out string result)
    {
        string? ip = context.GetCallerConnection()?.RemoteIpAddress?.ToString();
        if (ip == null)
        {
            result = string.Empty;
            return false;
        }
        result = ip;
        return true;
    }

    public static Dictionary<string, string> GetCallerQueryVariables(this HubCallerContext context)
    {
        var result = context.GetCallerHttpContext()?.HttpContext?.Request.Query.ConvertToDictionary(kv => kv.Key, kv => kv.Value.FirstOrDefault() ?? string.Empty);
        return result ?? new Dictionary<string, string>();
    }

    public static IHttpConnectionFeature? GetCallerConnection(this HubCallerContext context) => context.Features.Get<IHttpConnectionFeature>();
    public static IHttpContextFeature? GetCallerHttpContext(this HubCallerContext context) => context.Features.Get<IHttpContextFeature>();
}
