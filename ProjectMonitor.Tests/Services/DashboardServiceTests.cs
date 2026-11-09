using ProjectMonitor.Domain.DTOs.Configuration;
using ProjectMonitor.Domain.DTOs.Dashboard;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Interfaces.Helpers;
using ProjectMonitor.Domain.Services.Dashboard;

namespace ProjectMonitor.Tests.Services
{
    public class DashboardServiceTests
    {
        private sealed class FakeState(params ProjectSnapshot[] projects) : IDashboardStateHelper
        {
            public event Action? Changed;
            public DateTime? LastGitHubPollUtc => TestData.Now;
            public DateTime? LastUptimeCheckUtc => TestData.Now;
            public IReadOnlyList<ProjectSnapshot> GetProjects() => projects;
            public ProjectSnapshot? GetProject(string slug) => projects.FirstOrDefault(x => x.Project.Slug == slug);
            public void UpdateGitHub(string slug, GitHubSnapshot? snapshot, string? error) { }
            public void MarkGitHubPollComplete() => Changed?.Invoke();
            public void UpdateEndpoints(string slug, IReadOnlyList<EndpointStatus> endpoints) { }
            public void MarkUptimeCheckComplete() => Changed?.Invoke();
        }

        private static ProjectSnapshot Project(string slug, IReadOnlyList<EndpointStatus>? endpoints = null, GitHubSnapshot? github = null)
        {
            ProjectConfig config = new() { Slug = slug, Name = slug, Repository = slug };
            return new ProjectSnapshot(config, endpoints ?? [], github, github is null ? null : DateTime.UtcNow, null);
        }

        private static readonly ProjectSnapshot Healthy = Project("healthy", [TestData.Endpoint(true)], TestData.GitHub([TestData.Run("CI", "success")]));
        private static readonly ProjectSnapshot Down = Project("down", [TestData.Endpoint(false, error: "timeout")]);
        private static readonly ProjectSnapshot Vulnerable = Project("vulnerable", github: TestData.GitHub(alerts: [TestData.Alert("critical"), TestData.Alert("high"), TestData.Alert("low")]));

        [Test]
        public void GetAtAGlance_CountsProjectsServicesAndVulnerabilities()
        {
            DashboardService service = new(new FakeState(Healthy, Down, Vulnerable));

            SummaryTilesDto tiles = service.GetAtAGlance().Tiles;

            Assert.Multiple(() =>
            {
                Assert.That(tiles.TotalProjects, Is.EqualTo(3));
                Assert.That(tiles.HealthyProjects, Is.EqualTo(1));
                Assert.That(tiles.CriticalProjects, Is.EqualTo(2));
                Assert.That(tiles.ServicesUp, Is.EqualTo(1));
                Assert.That(tiles.ServicesDown, Is.EqualTo(1));
                Assert.That(tiles.CriticalVulnerabilities, Is.EqualTo(1));
                Assert.That(tiles.HighVulnerabilities, Is.EqualTo(1));
                Assert.That(tiles.OtherVulnerabilities, Is.EqualTo(1));
            });
        }

        [Test]
        public void GetAtAGlance_WorstProjectsFirst()
        {
            DashboardService service = new(new FakeState(Healthy, Down, Vulnerable));

            AtAGlanceDto result = service.GetAtAGlance();

            Assert.That(result.Projects.Last().Slug, Is.EqualTo("healthy"));
            Assert.That(result.SummaryLine, Is.EqualTo("2 projects need attention now"));
        }

        [Test]
        public void GetAtAGlance_NeedsAttentionOnlyHasWarningsAndCritical()
        {
            DashboardService service = new(new FakeState(Vulnerable));

            AtAGlanceDto result = service.GetAtAGlance();

            Assert.That(result.NeedsAttention.Select(x => x.Issue.Level), Is.All.GreaterThanOrEqualTo(HealthLevel.Warning));
            Assert.That(result.NeedsAttention, Has.Count.EqualTo(2), "critical and high, not low");
        }

        [Test]
        public void GetProjectDetails_UnknownSlug_ReturnsNull()
        {
            DashboardService service = new(new FakeState(Healthy));

            Assert.That(service.GetProjectDetails("nope"), Is.Null);
            Assert.That(service.GetProjectDetails("healthy")?.Health.Level, Is.EqualTo(HealthLevel.Healthy));
        }

        [Test]
        public void DataChanged_ForwardsStateChanges()
        {
            FakeState state = new(Healthy);
            DashboardService service = new(state);
            int calls = 0;
            service.DataChanged += () => calls++;

            state.MarkUptimeCheckComplete();

            Assert.That(calls, Is.EqualTo(1));
        }
    }
}
