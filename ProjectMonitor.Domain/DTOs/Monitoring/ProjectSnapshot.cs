using ProjectMonitor.Domain.DTOs.Configuration;

namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    /// <summary>Everything known about one project right now. Immutable, so jobs can swap it while pages read it.</summary>
    public record ProjectSnapshot(
        ProjectConfig Project,
        IReadOnlyList<EndpointStatus> Endpoints,
        GitHubSnapshot? GitHub,
        DateTime? GitHubUpdatedUtc,
        string? GitHubError)
    {
        /// <summary>Results of heartbeat and reported-value checks (and any future check types).</summary>
        public IReadOnlyList<CheckStatus> Checks { get; init; } = [];

        /// <summary>The latest value of every key the project has reported.</summary>
        public IReadOnlyList<ReportedValueInfo> LatestValues { get; init; } = [];

        public DateTime? LastReportUtc { get; init; }
    }
}
