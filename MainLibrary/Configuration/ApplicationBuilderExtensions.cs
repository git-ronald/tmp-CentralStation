using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CentralStation.Data.Configuration
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task MigrateData(this IApplicationBuilder app)
        {
            var factory = app.ApplicationServices.GetService<IServiceScopeFactory>();
            if (factory == null)
            {
                return;
            }

            using var scope = factory.CreateAsyncScope();
            await scope.ServiceProvider.GetRequiredService<MainDbContext>().Database.MigrateAsync();
        }
    }
}
