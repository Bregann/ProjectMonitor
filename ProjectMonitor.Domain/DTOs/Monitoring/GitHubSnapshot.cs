namespace ProjectMonitor.Domain.DTOs.Monitoring
{
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
}
