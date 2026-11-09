using Hangfire;

namespace ProjectMonitor.Domain.Interfaces.Jobs
{
    // Hangfire reads job attributes from the interface it runs the job through, so they live here
    public interface IUptimeService
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(Attempts = 0)]
        Task CheckEndpoints();

        [AutomaticRetry(Attempts = 0)]
        Task PruneOldChecks();
    }
}
