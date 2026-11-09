using ProjectMonitor.Domain.Health;
using ProjectMonitor.Domain.State;

namespace ProjectMonitor.Tests.Health
{
    public class HealthEvaluatorTests
    {
        [Test]
        public void Evaluate_EverythingFine_IsHealthy()
        {
            ProjectSnapshot snapshot = TestData.Snapshot([TestData.Endpoint(true)], TestData.GitHub([TestData.Run("CI", "success")]));

            ProjectHealth health = HealthEvaluator.Evaluate(snapshot, TestData.Now);

            Assert.That(health.Level, Is.EqualTo(HealthLevel.Healthy));
            Assert.That(health.Issues, Is.Empty);
            Assert.That(health.HasData, Is.True);
        }

        [Test]
        public void Evaluate_NothingLoadedYet_HasNoData()
        {
            ProjectSnapshot snapshot = TestData.Snapshot([TestData.Endpoint(null)]);

            ProjectHealth health = HealthEvaluator.Evaluate(snapshot with { Endpoints = [TestData.Endpoint(null) with { LastCheckedUtc = null }] }, TestData.Now);

            Assert.That(health.HasData, Is.False);
        }

        [Test]
        public void Evaluate_EndpointDown_IsCritical()
        {
            ProjectSnapshot snapshot = TestData.Snapshot([TestData.Endpoint(false, error: "No response within 10s")]);

            ProjectHealth health = HealthEvaluator.Evaluate(snapshot, TestData.Now);

            Assert.That(health.Level, Is.EqualTo(HealthLevel.Critical));
            Assert.That(health.Issues[0].Message, Is.EqualTo("API is down (No response within 10s)"));
            Assert.That(health.Issues[0].Key, Is.EqualTo("down:orbit/API"));
        }

        [Test]
        public void Evaluate_UpButLowUptime_IsWarning()
        {
            ProjectSnapshot snapshot = TestData.Snapshot([TestData.Endpoint(true, uptime24h: 97.5)]);

            ProjectHealth health = HealthEvaluator.Evaluate(snapshot, TestData.Now);

            Assert.That(health.Level, Is.EqualTo(HealthLevel.Warning));
            Assert.That(health.Issues[0].Message, Does.Contain("97.5%"));
        }

        [TestCase("Build and Publish Docker Image", HealthLevel.Critical)]
        [TestCase("Renovate", HealthLevel.Critical)]
        [TestCase("CI", HealthLevel.Warning)]
        [TestCase("Release Android (Orbit)", HealthLevel.Warning)]
        public void Evaluate_FailedWorkflowOnDefaultBranch_UsesWorkflowSeverity(string workflow, HealthLevel expected)
        {
            ProjectSnapshot snapshot = TestData.Snapshot(github: TestData.GitHub([TestData.Run(workflow, "failure")]));

            ProjectHealth health = HealthEvaluator.Evaluate(snapshot, TestData.Now);

            Assert.That(health.Level, Is.EqualTo(expected));
        }

        [TestCase("critical", HealthLevel.Critical)]
        [TestCase("high", HealthLevel.Warning)]
        [TestCase("medium", HealthLevel.Info)]
        [TestCase("low", HealthLevel.Info)]
        public void Evaluate_SecurityAlerts_MapSeverity(string severity, HealthLevel expected)
        {
            ProjectSnapshot snapshot = TestData.Snapshot(github: TestData.GitHub(alerts: [TestData.Alert(severity), TestData.Alert(severity)]));

            ProjectHealth health = HealthEvaluator.Evaluate(snapshot, TestData.Now);

            Assert.That(health.Level, Is.EqualTo(expected));
            Assert.That(health.Issues.Single().Message, Is.EqualTo($"2 {severity} vulnerabilities"));
        }

        [Test]
        public void Evaluate_CantReadSecurityAlerts_IsInfoNotCritical()
        {
            ProjectSnapshot snapshot = TestData.Snapshot(github: TestData.GitHub(alertsError: "no access"));

            ProjectHealth health = HealthEvaluator.Evaluate(snapshot, TestData.Now);

            Assert.That(health.Level, Is.EqualTo(HealthLevel.Info));
        }

        [Test]
        public void Evaluate_RenovatePrsAndExpoIssue_AreInfo()
        {
            PullRequestInfo renovatePr = new(5, "Update eslint", "u", "renovate[bot]", "renovate/eslint-10.x", false, TestData.Now);
            PullRequestInfo myPr = new(6, "Feature", "u", "Bregann", "feature/x", false, TestData.Now);
            IssueInfo expo = new(7, "Expo updates needed for orbit.app", "u", TestData.Now);

            ProjectSnapshot snapshot = TestData.Snapshot(github: TestData.GitHub(pullRequests: [renovatePr, myPr], expoIssues: [expo]));

            ProjectHealth health = HealthEvaluator.Evaluate(snapshot, TestData.Now);

            Assert.That(health.Level, Is.EqualTo(HealthLevel.Info));
            Assert.That(health.Issues.Select(x => x.Message), Is.EquivalentTo(new[] { "1 dependency update PR waiting for review", "Expo updates needed for orbit.app" }));
        }

        [Test]
        public void Evaluate_GitHubErrorWithRecentGoodData_IsIgnored()
        {
            ProjectSnapshot snapshot = TestData.Snapshot(github: TestData.GitHub(), githubError: "timeout", githubUpdated: TestData.Now.AddMinutes(-5));

            ProjectHealth health = HealthEvaluator.Evaluate(snapshot, TestData.Now);

            Assert.That(health.Level, Is.EqualTo(HealthLevel.Healthy));
        }

        [Test]
        public void Evaluate_GitHubErrorAndStaleData_IsWarning()
        {
            ProjectSnapshot snapshot = TestData.Snapshot(github: TestData.GitHub(), githubError: "timeout", githubUpdated: TestData.Now.AddMinutes(-30));

            ProjectHealth health = HealthEvaluator.Evaluate(snapshot, TestData.Now);

            Assert.That(health.Level, Is.EqualTo(HealthLevel.Warning));
        }

        [Test]
        public void Evaluate_IssuesAreOrderedMostSevereFirst()
        {
            ProjectSnapshot snapshot = TestData.Snapshot(
                [TestData.Endpoint(false)],
                TestData.GitHub([TestData.Run("CI", "failure")], [TestData.Alert("low")]));

            ProjectHealth health = HealthEvaluator.Evaluate(snapshot, TestData.Now);

            Assert.That(health.Issues.Select(x => x.Level), Is.Ordered.Descending);
        }
    }
}
