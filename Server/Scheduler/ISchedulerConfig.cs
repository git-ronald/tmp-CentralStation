using CoreLibrary.SchedulerService;

namespace CentralStation.Server.Scheduler
{
    internal interface ISchedulerConfig<TKey> where TKey : notnull
    {
        Dictionary<TKey, SchedulerTaskList> Schedule { get; }
    }
}