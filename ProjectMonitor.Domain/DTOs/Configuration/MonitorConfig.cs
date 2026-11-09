namespace ProjectMonitor.Domain.DTOs.Configuration
{
    /// <summary>
    /// Bound from the "Monitor" section of appsettings.json: which projects to watch and how.
    /// Secrets (tokens, passwords) live in the EnvironmentalSettings table instead.
    /// </summary>
    public class MonitorConfig
    {
        public const string SectionName = "Monitor";

        /// <summary>GitHub user or organisation that owns the repositories.</summary>
        public string GitHubOwner { get; set; } = "Bregann";

        public int UptimeTimeoutSeconds { get; set; } = 10;

        public int UptimeRetentionDays { get; set; } = 30;

        /// <summary>A service only counts as down after this many failed checks in a row, to ignore one-off blips.</summary>
        public int FailuresBeforeDown { get; set; } = 2;

        public List<ProjectConfig> Projects { get; set; } = [];
    }
}
