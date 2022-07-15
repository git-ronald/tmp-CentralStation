using Microsoft.AspNetCore.Components.Authorization;

namespace CentralStation.Client.Extensions;

public static class AuthorizationExtensions
{
    public static async Task<bool> IsAuthenticated(this AuthenticationStateProvider authenticationState)
    {
        AuthenticationState? authState = await authenticationState.GetAuthenticationStateAsync();
        if (authState == null)
        {
            return false;
        }
        return authState.User.Identity?.IsAuthenticated ?? false;
    }
}
