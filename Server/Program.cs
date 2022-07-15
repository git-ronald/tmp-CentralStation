using CentralStation.Data.Configuration;
using CentralStation.Server;
using CentralStation.Server.ConstantValues;
using CentralStation.Server.Scheduler;
using CentralStation.Server.Services;
using CoreLibrary;
using CoreLibrary.SchedulerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2c"));

builder.Services.Configure<JwtBearerOptions>(
    JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters.NameClaimType = "name";
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2cPeers"), AuthenticationConstants.PeerScheme);

builder.Services.Configure<JwtBearerOptions>(
    AuthenticationConstants.PeerScheme, options =>
    {
        options.Events.OnTokenValidated = ctx =>
        {
            // TODO: do some checks here?
            return Task.CompletedTask;
        };
    });

builder.Services.AddHostedService<ConsumeScopedServiceHostedService>();
builder.Services.AddScoped<ISchedulerService, SchedulerService>();
builder.Services.AddScoped<ISchedulerConfig<TimeSpan>, FixedTimeSchedulerConfig>(); // DefaultScheduleConfig<TimeSpan>>();
builder.Services.AddScoped<ISchedulerConfig<TimeCompartments>, TimeCompartmentSchedulerConfig>();

builder.Services.AddTransient<IPeerService, PeerService>();

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddMainLibrary();

var app = builder.Build();

await app.MigrateData();
app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();

app.MapHub<MainHub>("/mainhub");

app.MapFallbackToFile("index.html");

app.Run();
