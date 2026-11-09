namespace ProjectMonitor.Domain.Configuration
{
    /// <summary>
    /// Bound from the "Monitor" configuration section (appsettings.json, or env vars such as Monitor__GitHubToken).
    /// </summary>
    public class MonitorOptions
    {
        public const string SectionName = "Monitor";

        /// <summary>GitHub user or organisation that owns the repositories.</summary>
        public string GitHubOwner { get; set; } = "Bregann";

        /// <summary>Read-only fine-grained token: Actions, Contents, Issues, Pull requests, Dependabot alerts (all read).</summary>
        public string GitHubToken { get; set; } = "";

        /// <summary>Password for the dashboard login. Required outside Development.</summary>
        public string DashboardPassword { get; set; } = "";

        public int GitHubPollSeconds { get; set; } = 120;

        public int UptimeCheckSeconds { get; set; } = 60;

        public int UptimeTimeoutSeconds { get; set; } = 10;

        public int UptimeRetentionDays { get; set; } = 30;

        /// <summary>A service only counts as down after this many failed checks in a row, to ignore one-off blips.</summary>
        public int FailuresBeforeDown { get; set; } = 2;

        public AlertOptions Alerts { get; set; } = new();

        public List<ProjectOptions> Projects { get; set; } = [];
    }

    public class ProjectOptions
    {
        /// <summary>URL-safe identifier, used for the project's page (/projects/{slug}).</summary>
        public string Slug { get; set; } = "";

        public string Name { get; set; } = "";

        public string? Description { get; set; }

        /// <summary>Repository name under <see cref="MonitorOptions.GitHubOwner"/>. Leave empty for projects without a repo.</summary>
        public string? Repository { get; set; }

        public List<EndpointOptions> Endpoints { get; set; } = [];
    }

    public class EndpointOptions
    {
        public string Name { get; set; } = "";

        public string Url { get; set; } = "";

        /// <summary>
        /// Status codes that count as "up". When empty, any response below 500 counts as up:
        /// an API answering 401/404 on its root is still running.
        /// </summary>
        public List<int> ExpectedStatusCodes { get; set; } = [];
    }

    /// <summary>Telegram alerts sent through CommsSender. Alerts are off unless all three values are set.</summary>
    public class AlertOptions
    {
        public string CommsSenderUrl { get; set; } = "";

        /// <summary>CommsSender's server key (CommsSenderApiKey), not the app key.</summary>
        public string CommsSenderApiKey { get; set; } = "";

        public long TelegramChatId { get; set; }

        public bool IsConfigured => !string.IsNullOrWhiteSpace(CommsSenderUrl) && !string.IsNullOrWhiteSpace(CommsSenderApiKey) && TelegramChatId != 0;
    }
}
