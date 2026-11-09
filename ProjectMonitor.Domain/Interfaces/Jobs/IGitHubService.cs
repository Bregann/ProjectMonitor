using Hangfire;

namespace ProjectMonitor.Domain.Interfaces.Jobs
{
    // Hangfire reads job attributes from the interface it runs the job through, so they live here
    public interface IGitHubService
    {
        [DisableConcurrentExecution(120)]
        [AutomaticRetry(Attempts = 0)]
        Task PollRepositories();
    }
}
