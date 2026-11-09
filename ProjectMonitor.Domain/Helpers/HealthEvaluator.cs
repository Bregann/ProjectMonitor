using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.Helpers
{
    /// <summary>
    /// Turns a project's snapshot into a traffic light plus a list of reasons.
    /// Critical = something is broken now (service down, critical vulnerability, deploys failing).
    /// Warning = needs attention soon. Info = worth knowing, no action needed yet.
    /// </summary>
    public static class HealthEvaluator
    {
        public static readonly TimeSpan GitHubStaleAfter = TimeSpan.FromMinutes(15);
        public const double LowUptimeThreshold = 99.0;

        public static ProjectHealth Evaluate(ProjectSnapshot project, DateTime nowUtc)
        {
            List<HealthIssue> issues = [];

            AddEndpointIssues(project, issues);
            AddCheckIssues(project, issues);
            AddGitHubIssues(project, nowUtc, issues);

            bool hasData = project.Endpoints.Any(x => x.LastCheckedUtc is not null) || project.GitHub is not null || project.Checks.Count > 0;
            HealthLevel level = issues.Count == 0 ? HealthLevel.Healthy : issues.Max(x => x.Level);

            return new ProjectHealth(level, issues.OrderByDescending(x => x.Level).ToList(), hasData);
        }

        private static void AddEndpointIssues(ProjectSnapshot project, List<HealthIssue> issues)
        {
            foreach (EndpointStatus endpoint in project.Endpoints)
            {
                if (endpoint.IsUp == false)
                {
                    string detail = endpoint.Error ?? (endpoint.StatusCode is int code ? $"HTTP {code}" : "no response");
                    issues.Add(new(
                        $"down:{endpoint.Key}",
                        endpoint.FailureLevel,
                        "Uptime",
                        $"{endpoint.Name} is down ({detail})",
                        endpoint.Url));
                }
                else if (endpoint.Uptime24h is double uptime && uptime < LowUptimeThreshold)
                {
                    issues.Add(new(
                        $"flaky:{endpoint.Key}",
                        HealthLevel.Warning,
                        "Uptime",
                        $"{endpoint.Name} uptime is {uptime:0.##}% over the last 24 hours",
                        endpoint.Url));
                }
            }
        }

        private static void AddCheckIssues(ProjectSnapshot project, List<HealthIssue> issues)
        {
            foreach (CheckStatus check in project.Checks.Where(x => x.Level > HealthLevel.Healthy))
            {
                issues.Add(new(
                    $"check:{check.CheckId}",
                    check.Level,
                    "Checks",
                    $"{check.Name}: {check.Message}",
                    $"/projects/{project.Project.Slug}"));
            }
        }

        private static void AddGitHubIssues(ProjectSnapshot project, DateTime nowUtc, List<HealthIssue> issues)
        {
            if (string.IsNullOrWhiteSpace(project.Project.Repository))
            {
                return;
            }

            if (project.GitHubError is not null && (project.GitHubUpdatedUtc is null || nowUtc - project.GitHubUpdatedUtc > GitHubStaleAfter))
            {
                issues.Add(new($"github-error:{project.Project.Slug}", HealthLevel.Warning, "GitHub", project.GitHubError, null));
            }

            GitHubSnapshot? github = project.GitHub;

            if (github is null)
            {
                return;
            }

            foreach (WorkflowRunInfo run in github.LatestRunPerWorkflow.Where(x => x.IsFailure))
            {
                issues.Add(new(
                    $"workflow:{project.Project.Slug}:{run.WorkflowName}",
                    IsCriticalWorkflow(run.WorkflowName) ? HealthLevel.Critical : HealthLevel.Warning,
                    "Actions",
                    $"\"{run.WorkflowName}\" failed on {github.DefaultBranch}",
                    run.Url));
            }

            AddSecurityIssues(project, github, issues);

            foreach (IssueInfo expo in github.ExpoIssues)
            {
                issues.Add(new($"expo:{project.Project.Slug}", HealthLevel.Info, "Expo", expo.Title, expo.Url));
            }

            List<PullRequestInfo> waiting = github.OpenPullRequests.Where(x => x.IsRenovate).ToList();

            if (waiting.Count > 0)
            {
                issues.Add(new(
                    $"renovate-prs:{project.Project.Slug}",
                    HealthLevel.Info,
                    "Updates",
                    waiting.Count == 1 ? "1 dependency update PR waiting for review" : $"{waiting.Count} dependency update PRs waiting for review",
                    $"{github.RepositoryUrl}/pulls?q=is%3Apr+is%3Aopen+head%3Arenovate"));
            }
        }

        private static void AddSecurityIssues(ProjectSnapshot project, GitHubSnapshot github, List<HealthIssue> issues)
        {
            if (github.SecurityAlertsError is not null)
            {
                issues.Add(new($"security-access:{project.Project.Slug}", HealthLevel.Info, "Security", github.SecurityAlertsError, null));
                return;
            }

            string alertsUrl = $"{github.RepositoryUrl}/security/dependabot";

            foreach (IGrouping<string, SecurityAlertInfo> severity in github.SecurityAlerts.GroupBy(x => x.Severity))
            {
                HealthLevel level = severity.Key switch
                {
                    "critical" => HealthLevel.Critical,
                    "high" => HealthLevel.Warning,
                    _ => HealthLevel.Info,
                };

                int count = severity.Count();
                issues.Add(new(
                    $"security:{project.Project.Slug}:{severity.Key}",
                    level,
                    "Security",
                    $"{count} {severity.Key} {(count == 1 ? "vulnerability" : "vulnerabilities")}",
                    alertsUrl));
            }
        }

        /// <summary>
        /// A failing deploy means the live site isn't getting updates; a failing Renovate run usually means
        /// its token expired and nothing is being updated at all. Other failures (CI, releases) are warnings.
        /// </summary>
        public static bool IsCriticalWorkflow(string workflowName)
        {
            return workflowName.Contains("docker", StringComparison.OrdinalIgnoreCase)
                || workflowName.Contains("deploy", StringComparison.OrdinalIgnoreCase)
                || workflowName.Contains("renovate", StringComparison.OrdinalIgnoreCase);
        }
    }
}
