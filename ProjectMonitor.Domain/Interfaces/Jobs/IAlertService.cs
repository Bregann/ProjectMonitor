using Hangfire;

namespace ProjectMonitor.Domain.Interfaces.Jobs
{
    // Hangfire reads job attributes from the interface it runs the job through, so they live here
    public interface IAlertService
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(Attempts = 0)]
        Task CheckAndSendAlerts();
    }
}
