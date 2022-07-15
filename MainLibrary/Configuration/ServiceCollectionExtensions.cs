using Microsoft.Extensions.DependencyInjection;

namespace CentralStation.Data.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMainLibrary(this IServiceCollection services)
        {
            return services.AddDbContext<MainDbContext>(ServiceLifetime.Transient);
        }
    }
}
