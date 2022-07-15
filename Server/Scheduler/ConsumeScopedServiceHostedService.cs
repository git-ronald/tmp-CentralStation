using CoreLibrary;
using CoreLibrary.SchedulerService;

namespace CentralStation.Server.Scheduler
{
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio#backgroundservice-base-class

    public class ConsumeScopedServiceHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ConsumeScopedServiceHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            ISchedulerService scheduleService = scope.ServiceProvider.GetRequiredService<ISchedulerService>();

            var fixedTimeSchedule = scope.ServiceProvider.GetRequiredService<ISchedulerConfig<TimeSpan>>();
            var compartmentSchedule = scope.ServiceProvider.GetRequiredService<ISchedulerConfig<TimeCompartments>>();
            await scheduleService.Start(stoppingToken, fixedTimeSchedule.Schedule, compartmentSchedule.Schedule);
        }
    }
}
