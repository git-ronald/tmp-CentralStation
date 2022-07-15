using CentralStation.Server.Services;
using CoreLibrary;
using CoreLibrary.Helpers;
using CoreLibrary.SchedulerService;

namespace CentralStation.Server.Scheduler
{
    internal class TimeCompartmentSchedulerConfig : ISchedulerConfig<TimeCompartments>
    {
        private readonly IPeerService _peerService;

        public TimeCompartmentSchedulerConfig(IPeerService peerService)
        {
            _peerService = peerService;

            Schedule = new Dictionary<TimeCompartments, SchedulerTaskList>();
            Schedule.Ensure(TimeCompartments.EveryHour).Add(cancellation => _peerService.DeleteExpired(cancellation));
        }

        public Dictionary<TimeCompartments, SchedulerTaskList> Schedule { get; }
    }
}
