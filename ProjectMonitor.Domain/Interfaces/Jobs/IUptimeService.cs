namespace ProjectMonitor.Domain.Interfaces.Jobs
{
    public interface IUptimeService
    {
        Task CheckEndpoints();
        Task PruneOldChecks();
    }
}
