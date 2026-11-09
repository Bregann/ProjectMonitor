using Hangfire;
using ProjectMonitor.Domain.Interfaces.Jobs;

namespace ProjectMonitor.Domain.Helpers
{
    public class HangfireJobSetup
    {
        public static void SetupRecurringJobs()
        {
            RecurringJob.AddOrUpdate<IUptimeService>("CheckUptime", service => service.CheckEndpoints(), Cron.Minutely);
            RecurringJob.AddOrUpdate<IUptimeService>("PruneUptimeChecks", service => service.PruneOldChecks(), Cron.Hourly);
            RecurringJob.AddOrUpdate<IGitHubService>("PollGitHub", service => service.PollRepositories(), "*/2 * * * *");
            RecurringJob.AddOrUpdate<IAlertService>("CheckAlerts", service => service.CheckAndSendAlerts(), "*/30 * * * * *");

            // Fill the dashboard straight away instead of waiting for the first schedule
            BackgroundJob.Enqueue<IUptimeService>(service => service.CheckEndpoints());
            BackgroundJob.Enqueue<IGitHubService>(service => service.PollRepositories());
        }
    }
}
