using ProjectMonitor.Domain.GitHub;
using ProjectMonitor.Domain.State;

namespace ProjectMonitor.Tests.GitHub
{
    public class GitHubSnapshotBuilderTests
    {
        private static readonly GitHubRepository Repo = new("https://github.com/Bregann/Orbit", "main");

        private static GitHubWorkflowRun Run(long id, string name, string branch, string @event, string status, string? conclusion, int minutesAgo)
        {
            DateTime created = TestData.Now.AddMinutes(-minutesAgo);
            return new GitHubWorkflowRun(id, 1, name, $"title {id}", branch, @event, status, conclusion, $"https://github.com/run/{id}", created, created, new GitHubUser("Bregann"));
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
            List<GitHubIssue> issues =
            [
                new(1, "Expo updates needed for orbit.app", "u1", TestData.Now, [new GitHubLabel("expo-updates")], null),
                new(2, "Dependency Dashboard", "u2", TestData.Now.AddDays(-3), [], null),
                new(3, "Dependency Dashboard", "u3", TestData.Now, [], null),
                new(4, "Some bug", "u4", TestData.Now, [], null),
            ];

            GitHubSnapshot snapshot = GitHubSnapshotBuilder.Build(Repo, [], [], issues, [], TestData.Now);

            Assert.That(snapshot.ExpoIssues.Select(x => x.Number), Is.EqualTo(new[] { 1 }));
            Assert.That(snapshot.DependencyDashboard?.Number, Is.EqualTo(3), "the most recently updated dashboard wins");
        }

        [Test]
        public void Build_SecurityAlerts_SortedBySeverityAndModerateBecomesMedium()
        {
            List<GitHubDependabotAlert> alerts =
            [
                Alert(1, "low"),
                Alert(2, "moderate"),
                Alert(3, "critical"),
            ];

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

        private static GitHubDependabotAlert Alert(int number, string severity)
        {
            return new GitHubDependabotAlert(
                number,
                $"https://github.com/alert/{number}",
                TestData.Now,
                new GitHubAlertDependency("package-lock.json", "transitive"),
                new GitHubSecurityAdvisory("Summary", severity),
                new GitHubSecurityVulnerability(new GitHubPackage("pkg", "npm"), severity, new GitHubPatchedVersion("1.0.1")));
        }
    }
}
