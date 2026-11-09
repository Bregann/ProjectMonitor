namespace ProjectMonitor.Domain.Interfaces.Jobs
{
    public interface IAlertService
    {
        Task CheckAndSendAlerts();
    }
}
