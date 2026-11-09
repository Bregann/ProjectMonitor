using Hangfire;

namespace ProjectMonitor.Domain.Interfaces.Jobs
{
    // Hangfire reads job attributes from the interface it runs the job through, so they live here
    public interface IChecksService
    {
        /// <summary>Evaluates every project's heartbeat and value checks against what they've reported.</summary>
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(Attempts = 0)]
        Task EvaluateAll();

        [AutomaticRetry(Attempts = 0)]
        Task EvaluateProject(int projectId);

        [AutomaticRetry(Attempts = 0)]
        Task PruneReportedValues();
    }
}
