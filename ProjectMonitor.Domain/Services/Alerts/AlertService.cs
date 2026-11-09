using Hangfire;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Helpers;
using ProjectMonitor.Domain.Interfaces.Helpers;
using ProjectMonitor.Domain.Interfaces.Jobs;

namespace ProjectMonitor.Domain.Services.Alerts
{
    /// <summary>
    /// Sends a Telegram message (via CommsSender) when a critical problem appears and again when it clears.
    /// Problems that already exist when the monitor starts are not re-announced, so redeploys don't spam.
    /// </summary>
    public class AlertService(IDashboardStateHelper state, AlertTracker tracker, ICommsSenderClient commsSender) : IAlertService
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckAndSendAlerts()
        {
            // Wait until both data sources have reported once, otherwise "no data yet" looks like "everything recovered"
            if (!commsSender.IsConfigured || state.LastGitHubPollUtc is null || state.LastUptimeCheckUtc is null)
            {
                return;
            }

            List<(string Project, HealthIssue Issue)> critical = state.GetProjects()
                .SelectMany(p => HealthEvaluator.Evaluate(p, DateTime.UtcNow).Issues
                    .Where(i => i.Level == HealthLevel.Critical)
                    .Select(i => (p.Project.Name, i)))
                .ToList();

            foreach (AlertMessage message in tracker.Update(critical))
            {
                await commsSender.SendTelegramMessage(message.Text);
            }
        }
    }
}
