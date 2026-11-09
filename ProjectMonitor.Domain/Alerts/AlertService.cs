using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectMonitor.Domain.Configuration;
using ProjectMonitor.Domain.Health;
using ProjectMonitor.Domain.State;
using System.Net.Http.Json;

namespace ProjectMonitor.Domain.Alerts
{
    /// <summary>
    /// Sends a Telegram message (via CommsSender) when a critical problem appears and again when it clears.
    /// Problems that already exist when the monitor starts are not re-announced, so redeploys don't spam.
    /// </summary>
    public class AlertService(
        DashboardState state,
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<MonitorOptions> options,
        ILogger<AlertService> logger) : BackgroundService
    {
        public const string HttpClientName = "commssender";

        private readonly AlertTracker _tracker = new();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!options.CurrentValue.Alerts.IsConfigured)
            {
                logger.LogInformation("Telegram alerts are off: set Monitor:Alerts:CommsSenderUrl, CommsSenderApiKey and TelegramChatId to turn them on");
                return;
            }

            using PeriodicTimer timer = new(TimeSpan.FromSeconds(30));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                // Wait until both data sources have reported once, otherwise "no data yet" looks like "everything recovered"
                if (state.LastGitHubPollUtc is null || state.LastUptimeCheckUtc is null)
                {
                    continue;
                }

                List<(string Project, HealthIssue Issue)> critical = state.GetProjects()
                    .SelectMany(p => HealthEvaluator.Evaluate(p, DateTime.UtcNow).Issues
                        .Where(i => i.Level == HealthLevel.Critical)
                        .Select(i => (p.Project.Name, i)))
                    .ToList();

                foreach (AlertMessage message in _tracker.Update(critical))
                {
                    await SendAsync(message.Text, stoppingToken);
                }
            }
        }

        private async Task SendAsync(string text, CancellationToken cancellationToken)
        {
            AlertOptions alerts = options.CurrentValue.Alerts;

            try
            {
                HttpClient client = httpClientFactory.CreateClient(HttpClientName);
                using HttpRequestMessage request = new(HttpMethod.Post, $"{alerts.CommsSenderUrl.TrimEnd('/')}/api/Message/SendTelegramMessage")
                {
                    Content = JsonContent.Create(new { chatId = alerts.TelegramChatId, messageText = text }),
                };
                request.Headers.Add("X-CommsSender-ApiKey", alerts.CommsSenderApiKey);

                using HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex, "Couldn't send Telegram alert: {Text}", text);
            }
        }
    }

    public record AlertMessage(string Key, string Text);

    /// <summary>Works out which alerts to send by comparing this check's critical issues with the last one's.</summary>
    public class AlertTracker
    {
        private Dictionary<string, (string Project, string Message)>? _active;

        public IReadOnlyList<AlertMessage> Update(IReadOnlyList<(string Project, HealthIssue Issue)> critical)
        {
            Dictionary<string, (string Project, string Message)> current = critical
                .GroupBy(x => x.Issue.Key)
                .ToDictionary(g => g.Key, g => (g.First().Project, g.First().Issue.Message));

            // First run: remember what's already broken without announcing it
            if (_active is null)
            {
                _active = current;
                return [];
            }

            List<AlertMessage> messages = [];

            foreach ((string key, (string project, string message)) in current.Where(x => !_active.ContainsKey(x.Key)))
            {
                messages.Add(new(key, $"🔴 {project}: {message}"));
            }

            foreach ((string key, (string project, string message)) in _active.Where(x => !current.ContainsKey(x.Key)))
            {
                messages.Add(new(key, $"✅ {project}: resolved: {message}"));
            }

            _active = current;
            return messages;
        }
    }
}
