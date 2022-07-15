using CoreLibrary.SchedulerService;

namespace CentralStation.Server.Scheduler
{
    internal class FixedTimeSchedulerConfig : ISchedulerConfig<TimeSpan>
    {
        // TODO: MainHub should be restarted once a day (to purge dead client registrations).
        // Assuming that all clients (back and front) should then automatically reconnect themselves using their scheduler.

        public FixedTimeSchedulerConfig()
        {
            Schedule = new Dictionary<TimeSpan, SchedulerTaskList>();
        }

        public Dictionary<TimeSpan, SchedulerTaskList> Schedule { get; }
    }
}
