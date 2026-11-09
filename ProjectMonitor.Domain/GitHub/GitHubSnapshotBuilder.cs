using ProjectMonitor.Domain.State;

namespace ProjectMonitor.Domain.GitHub
{
    public static class GitHubSnapshotBuilder
    {
        public const string ExpoIssueLabel = "expo-updates";
        public const string DependencyDashboardTitle = "Dependency Dashboard";

        /// <summary>Workflows with no run on the default branch for this long are treated as removed (their old runs stay in GitHub's history).</summary>
        public static readonly TimeSpan InactiveWorkflowAfter = TimeSpan.FromDays(30);

        private static readonly Dictionary<string, int> SeverityOrder = new(StringComparer.OrdinalIgnoreCase)
        {
            ["critical"] = 0,
            ["high"] = 1,
            ["medium"] = 2,
            ["moderate"] = 2,
            ["low"] = 3,
        };

        public static GitHubSnapshot Build(
            GitHubRepository repository,
            IReadOnlyList<GitHubWorkflowRun> runs,
            IReadOnlyList<GitHubPullRequest> pullRequests,
            IReadOnlyList<GitHubIssue> issues,
            IReadOnlyList<GitHubDependabotAlert>? alerts,
            DateTime nowUtc)
        {
            List<WorkflowRunInfo> allRuns = runs.Select(ToRunInfo).OrderByDescending(x => x.CreatedUtc).ToList();

            // "Is main healthy?" = the most recent run of each workflow on the default branch, ignoring PR runs
            List<WorkflowRunInfo> latestPerWorkflow = allRuns
                .Where(x => x.Branch == repository.DefaultBranch && x.Event != "pull_request")
                .GroupBy(x => x.WorkflowName)
                .Select(g => g.First())
                .Where(x => nowUtc - x.CreatedUtc < InactiveWorkflowAfter)
                .OrderBy(x => x.WorkflowName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            List<IssueInfo> expoIssues = issues
                .Where(x => x.Labels?.Any(l => l.Name == ExpoIssueLabel) == true)
                .Select(ToIssueInfo)
                .ToList();

            IssueInfo? dependencyDashboard = issues
                .Where(x => x.Title == DependencyDashboardTitle)
                .OrderByDescending(x => x.UpdatedAt)
                .Select(ToIssueInfo)
                .FirstOrDefault();

            List<SecurityAlertInfo> securityAlerts = (alerts ?? [])
                .Select(ToAlertInfo)
                .OrderBy(x => SeverityRank(x.Severity))
                .ThenByDescending(x => x.CreatedUtc)
                .ToList();

            return new GitHubSnapshot(
                RepositoryUrl: repository.HtmlUrl,
                DefaultBranch: repository.DefaultBranch,
                LatestRunPerWorkflow: latestPerWorkflow,
                ActiveRuns: allRuns.Where(x => x.IsActive).ToList(),
                RecentRuns: allRuns.Take(20).ToList(),
                OpenPullRequests: pullRequests.Select(ToPullRequestInfo).OrderByDescending(x => x.CreatedUtc).ToList(),
                SecurityAlerts: securityAlerts,
                SecurityAlertsError: alerts is null ? "Can't read security alerts: give the GitHub token \"Dependabot alerts: Read\" and make sure alerts are enabled for the repo." : null,
                ExpoIssues: expoIssues,
                DependencyDashboard: dependencyDashboard);
        }

        public static int SeverityRank(string severity)
        {
            return SeverityOrder.TryGetValue(severity, out int rank) ? rank : 4;
        }

        private static WorkflowRunInfo ToRunInfo(GitHubWorkflowRun run)
        {
            return new WorkflowRunInfo(
                Id: run.Id,
                WorkflowName: run.Name ?? $"Workflow {run.WorkflowId}",
                Title: run.DisplayTitle ?? run.Name ?? "",
                Branch: run.HeadBranch ?? "",
                Event: run.Event,
                Status: run.Status,
                Conclusion: run.Conclusion,
                Url: run.HtmlUrl,
                CreatedUtc: DateTime.SpecifyKind(run.CreatedAt, DateTimeKind.Utc),
                UpdatedUtc: DateTime.SpecifyKind(run.UpdatedAt, DateTimeKind.Utc),
                Actor: run.Actor?.Login);
        }

        private static PullRequestInfo ToPullRequestInfo(GitHubPullRequest pr)
        {
            return new PullRequestInfo(
                Number: pr.Number,
                Title: pr.Title,
                Url: pr.HtmlUrl,
                Author: pr.User?.Login ?? "",
                Branch: pr.Head.Ref,
                IsDraft: pr.Draft,
                CreatedUtc: DateTime.SpecifyKind(pr.CreatedAt, DateTimeKind.Utc));
        }

        private static IssueInfo ToIssueInfo(GitHubIssue issue)
        {
            return new IssueInfo(issue.Number, issue.Title, issue.HtmlUrl, DateTime.SpecifyKind(issue.UpdatedAt, DateTimeKind.Utc));
        }

        private static SecurityAlertInfo ToAlertInfo(GitHubDependabotAlert alert)
        {
            string severity = alert.SecurityAdvisory?.Severity ?? alert.SecurityVulnerability?.Severity ?? "unknown";

            return new SecurityAlertInfo(
                Number: alert.Number,
                Severity: severity.ToLowerInvariant() == "moderate" ? "medium" : severity.ToLowerInvariant(),
                Summary: alert.SecurityAdvisory?.Summary ?? "",
                Package: alert.SecurityVulnerability?.Package?.Name ?? "unknown",
                Ecosystem: alert.SecurityVulnerability?.Package?.Ecosystem ?? "",
                ManifestPath: alert.Dependency?.ManifestPath,
                Relationship: alert.Dependency?.Relationship,
                FixedIn: alert.SecurityVulnerability?.FirstPatchedVersion?.Identifier,
                Url: alert.HtmlUrl,
                CreatedUtc: DateTime.SpecifyKind(alert.CreatedAt, DateTimeKind.Utc));
        }
    }
}
