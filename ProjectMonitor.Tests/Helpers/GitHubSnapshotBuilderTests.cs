using ProjectMonitor.Domain.DTOs.GitHub;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Helpers;

namespace ProjectMonitor.Tests.Helpers
{
    public class GitHubSnapshotBuilderTests
    {
        private static readonly GitHubRepositoryResponse Repo = new() { HtmlUrl = "https://github.com/Bregann/Orbit", DefaultBranch = "main" };

        private static GitHubWorkflowRun Run(long id, string name, string branch, string @event, string status, string? conclusion, int minutesAgo)
        {
            DateTime created = TestData.Now.AddMinutes(-minutesAgo);

            return new GitHubWorkflowRun
            {
                Id = id,
                WorkflowId = 1,
                Name = name,
                DisplayTitle = $"title {id}",
                HeadBranch = branch,
                Event = @event,
                Status = status,
                Conclusion = conclusion,
                HtmlUrl = $"https://github.com/run/{id}",
                CreatedAt = created,
                UpdatedAt = created,
                Actor = new GitHubUser { Login = "Bregann" }
            };
        }

        private static GitHubIssueResponse Issue(int number, string title, DateTime updated, params string[] labels)
        {
            return new GitHubIssueResponse
            {
                Number = number,
                Title = title,
                HtmlUrl = $"u{number}",
                UpdatedAt = updated,
                Labels = labels.Select(x => new GitHubLabel { Name = x }).ToList()
            };
        }

        private static GitHubDependabotAlertResponse Alert(int number, string severity)
        {
            return new GitHubDependabotAlertResponse
            {
                Number = number,
                HtmlUrl = $"https://github.com/alert/{number}",
                CreatedAt = TestData.Now,
                Dependency = new GitHubAlertDependency { ManifestPath = "package-lock.json", Relationship = "transitive" },
                SecurityAdvisory = new GitHubSecurityAdvisory { Summary = "Summary", Severity = severity },
                SecurityVulnerability = new GitHubSecurityVulnerability
                {
                    Package = new GitHubPackage { Name = "pkg", Ecosystem = "npm" },
                    Severity = severity,
                    FirstPatchedVersion = new GitHubPatchedVersion { Identifier = "1.0.1" }
                }
            };
        }

        [Test]
        public void Build_LatestRunPerWorkflow_UsesNewestDefaultBranchRunAndIgnoresPullRequests()
        {
            List<GitHubWorkflowRun> runs =
            [
                Run(1, "CI", "main", "push", "completed", "failure", 30),
                Run(2, "CI", "main", "push", "completed", "success", 10),
                Run(3, "CI", "renovate/x", "pull_request", "completed", "failure", 1),
                Run(4, "Build and Publish Docker Image", "main", "push", "completed", "success", 9),
            ];

            GitHubSnapshot snapshot = GitHubSnapshotBuilder.Build(Repo, runs, [], [], [], TestData.Now);

            Assert.That(snapshot.LatestRunPerWorkflow.Select(x => x.Id), Is.EquivalentTo(new long[] { 2, 4 }));
        }

        [Test]
        public void Build_LatestRunPerWorkflow_HidesWorkflowsWithNoRecentRun()
        {
            List<GitHubWorkflowRun> runs =
            [
                Run(1, "CI", "main", "push", "completed", "success", 10),
                Run(2, "Run C# formatting", "main", "push", "completed", "success", 60 * 24 * 40),
            ];

            GitHubSnapshot snapshot = GitHubSnapshotBuilder.Build(Repo, runs, [], [], [], TestData.Now);

            Assert.That(snapshot.LatestRunPerWorkflow.Select(x => x.WorkflowName), Is.EqualTo(new[] { "CI" }));
        }

        [Test]
        public void Build_ActiveRuns_IncludesQueuedAndInProgressOnAnyBranch()
        {
            List<GitHubWorkflowRun> runs =
            [
                Run(1, "CI", "renovate/x", "pull_request", "in_progress", null, 1),
                Run(2, "CI", "main", "push", "queued", null, 1),
                Run(3, "CI", "main", "push", "completed", "success", 5),
            ];

            GitHubSnapshot snapshot = GitHubSnapshotBuilder.Build(Repo, runs, [], [], [], TestData.Now);

            Assert.That(snapshot.ActiveRuns.Select(x => x.Id), Is.EquivalentTo(new long[] { 1, 2 }));
        }

        [Test]
        public void Build_FindsExpoIssuesAndDependencyDashboard()
        {
            List<GitHubIssueResponse> issues =
            [
                Issue(1, "Expo updates needed for orbit.app", TestData.Now, "expo-updates"),
                Issue(2, "Dependency Dashboard", TestData.Now.AddDays(-3)),
                Issue(3, "Dependency Dashboard", TestData.Now),
                Issue(4, "Some bug", TestData.Now),
            ];

            GitHubSnapshot snapshot = GitHubSnapshotBuilder.Build(Repo, [], [], issues, [], TestData.Now);

            Assert.That(snapshot.ExpoIssues.Select(x => x.Number), Is.EqualTo(new[] { 1 }));
            Assert.That(snapshot.DependencyDashboard?.Number, Is.EqualTo(3), "the most recently updated dashboard wins");
        }

        [Test]
        public void Build_SecurityAlerts_SortedBySeverityAndModerateBecomesMedium()
        {
            List<GitHubDependabotAlertResponse> alerts = [Alert(1, "low"), Alert(2, "moderate"), Alert(3, "critical")];

            GitHubSnapshot snapshot = GitHubSnapshotBuilder.Build(Repo, [], [], [], alerts, TestData.Now);

            Assert.That(snapshot.SecurityAlerts.Select(x => x.Severity), Is.EqualTo(new[] { "critical", "medium", "low" }));
            Assert.That(snapshot.SecurityAlertsError, Is.Null);
        }

        [Test]
        public void Build_NoAlertAccess_SetsErrorInsteadOfLookingSafe()
        {
            GitHubSnapshot snapshot = GitHubSnapshotBuilder.Build(Repo, [], [], [], null, TestData.Now);

            Assert.That(snapshot.SecurityAlerts, Is.Empty);
            Assert.That(snapshot.SecurityAlertsError, Does.Contain("Dependabot alerts"));
        }
    }
}
