using ProjectMonitor.Domain.DTOs.Monitoring;

namespace ProjectMonitor.Domain.DTOs.Dashboard
{
    public class ProjectRowDto
    {
        public required string Slug { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required ProjectHealth Health { get; set; }
        public required IReadOnlyList<EndpointStatus> Endpoints { get; set; }
        public double? AverageUptime24h { get; set; }
        public bool HasGitHubData { get; set; }
        public IReadOnlyList<WorkflowRunInfo> LatestRuns { get; set; } = [];
        public WorkflowRunInfo? LastDeploy { get; set; }
        public IReadOnlyList<SecurityAlertInfo> SecurityAlerts { get; set; } = [];
        public string? SecurityAlertsError { get; set; }
        public int? OpenPullRequests { get; set; }

        /// <summary>The most serious warning or critical issue, if any.</summary>
        public HealthIssue? TopIssue { get; set; }
    }
}
