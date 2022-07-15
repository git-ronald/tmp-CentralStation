using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CentralStation.Client;
using CentralStation.Client.Settings;
using CentralStation.Client.Services;
using CoreLibrary.SchedulerService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddHttpClient("CentralStation.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("CentralStation.ServerAPI"));

builder.Services.AddScoped<ISchedulerService, SchedulerService>();
builder.Services.AddScoped<IHubClientService, HubClientService>();
builder.Services.AddScoped<INavigationService, NavigationService>();
builder.Services.AddScoped<ICentralManagementService, CentralManagementService>();

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAdB2c", options.ProviderOptions.Authentication);

    foreach (string scope in builder.Configuration.GetSection("AppSettings").GetSection("AllowedScopes").Get<IEnumerable<string>>())
    {
        options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);
    }

    options.ProviderOptions.LoginMode = "redirect";
});

await builder.Build().RunAsync();
