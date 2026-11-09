using ProjectMonitor.Domain.Configuration;

namespace ProjectMonitor.Domain.State
{
    /// <summary>Everything the dashboard knows about one project right now.</summary>
    public record ProjectSnapshot(
        ProjectOptions Project,
        IReadOnlyList<EndpointStatus> Endpoints,
        GitHubSnapshot? GitHub,
        DateTime? GitHubUpdatedUtc,
        string? GitHubError);

    public record EndpointStatus(
        string Key,
        string Name,
        string Url,
        bool? IsUp,
        DateTime? LastCheckedUtc,
        int? StatusCode,
        int? ResponseTimeMs,
        string? Error,
        double? Uptime24h,
        double? Uptime7d,
        DateTime? DownSinceUtc,
        IReadOnlyList<UptimeBucket> History);

    /// <summary>One bar in the uptime history strip: share of successful checks in that period, or null if there were none.</summary>
    public record UptimeBucket(DateTime StartUtc, double? UptimePercent);

    public record GitHubSnapshot(
        string RepositoryUrl,
        string DefaultBranch,
        IReadOnlyList<WorkflowRunInfo> LatestRunPerWorkflow,
        IReadOnlyList<WorkflowRunInfo> ActiveRuns,
        IReadOnlyList<WorkflowRunInfo> RecentRuns,
        IReadOnlyList<PullRequestInfo> OpenPullRequests,
        IReadOnlyList<SecurityAlertInfo> SecurityAlerts,
        string? SecurityAlertsError,
        IReadOnlyList<IssueInfo> ExpoIssues,
        IssueInfo? DependencyDashboard);

    public record WorkflowRunInfo(
        long Id,
        string WorkflowName,
        string Title,
        string Branch,
        string Event,
        string Status,
        string? Conclusion,
        string Url,
        DateTime CreatedUtc,
        DateTime UpdatedUtc,
        string? Actor)
    {
        public bool IsActive => Status is "queued" or "in_progress" or "waiting" or "requested" or "pending";

        public bool IsFailure => Status == "completed" && Conclusion is "failure" or "timed_out" or "startup_failure";

        public bool IsSuccess => Status == "completed" && Conclusion == "success";
    }

    public record PullRequestInfo(
        int Number,
        string Title,
        string Url,
        string Author,
        string Branch,
        bool IsDraft,
        DateTime CreatedUtc)
    {
        public bool IsRenovate => Branch.StartsWith("renovate/", StringComparison.Ordinal);
    }

    public record SecurityAlertInfo(
        int Number,
        string Severity,
        string Summary,
        string Package,
        string Ecosystem,
        string? ManifestPath,
        string? Relationship,
        string? FixedIn,
        string Url,
        DateTime CreatedUtc);

    public record IssueInfo(int Number, string Title, string Url, DateTime UpdatedUtc);
}
