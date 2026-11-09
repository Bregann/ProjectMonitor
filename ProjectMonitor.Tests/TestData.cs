using ProjectMonitor.Domain.DTOs.Configuration;
using ProjectMonitor.Domain.DTOs.Monitoring;

namespace ProjectMonitor.Tests
{
    internal static class TestData
    {
        public static readonly DateTime Now = new(2026, 11, 9, 12, 0, 0, DateTimeKind.Utc);

        public static ProjectConfig Project(string? repository = "Orbit")
        {
            return new ProjectConfig { Slug = "orbit", Name = "Orbit", Repository = repository };
        }

        public static EndpointStatus Endpoint(bool? isUp, double? uptime24h = 100, string? error = null)
        {
            return new EndpointStatus("orbit/API", "API", "https://orbitapi.example", isUp, Now, isUp == true ? 200 : null, 50, error, uptime24h, 100, null, []);
        }

        public static WorkflowRunInfo Run(string workflow, string conclusion, string branch = "main", string @event = "push", string status = "completed", int minutesAgo = 5)
        {
            return new WorkflowRunInfo(1, workflow, workflow, branch, @event, status, conclusion, "https://github.com/run", Now.AddMinutes(-minutesAgo), Now.AddMinutes(-minutesAgo), "Bregann");
        }

        public static SecurityAlertInfo Alert(string severity)
        {
            return new SecurityAlertInfo(1, severity, "Bad thing", "pkg", "npm", "package-lock.json", "transitive", "1.2.3", "https://github.com/alert", Now);
        }

        public static GitHubSnapshot GitHub(
            IReadOnlyList<WorkflowRunInfo>? latestRuns = null,
            IReadOnlyList<SecurityAlertInfo>? alerts = null,
            string? alertsError = null,
            IReadOnlyList<PullRequestInfo>? pullRequests = null,
            IReadOnlyList<IssueInfo>? expoIssues = null)
        {
            return new GitHubSnapshot(
                "https://github.com/Bregann/Orbit",
                "main",
                latestRuns ?? [],
                [],
                latestRuns ?? [],
                pullRequests ?? [],
                alerts ?? [],
                alertsError,
                expoIssues ?? [],
                null);
        }

        public static ProjectSnapshot Snapshot(IReadOnlyList<EndpointStatus>? endpoints = null, GitHubSnapshot? github = null, string? githubError = null, DateTime? githubUpdated = null)
        {
            return new ProjectSnapshot(Project(), endpoints ?? [], github, githubUpdated ?? (github is null ? null : Now), githubError);
        }
    }
}
