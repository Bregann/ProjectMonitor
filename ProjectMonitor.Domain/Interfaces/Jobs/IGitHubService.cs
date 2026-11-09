namespace ProjectMonitor.Domain.Interfaces.Jobs
{
    public interface IGitHubService
    {
        Task PollRepositories();
    }
}
