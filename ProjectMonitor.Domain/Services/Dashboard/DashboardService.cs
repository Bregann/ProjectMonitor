using ProjectMonitor.Domain.DTOs.Dashboard;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Helpers;
using ProjectMonitor.Domain.Interfaces.Api.Dashboard;
using ProjectMonitor.Domain.Interfaces.Helpers;

namespace ProjectMonitor.Domain.Services.Dashboard
{
    public class DashboardService(IDashboardStateHelper state) : IDashboardService
    {
        public event Action? DataChanged
        {
            add => state.Changed += value;
            remove => state.Changed -= value;
        }

        public AtAGlanceDto GetAtAGlance()
        {
            DateTime nowUtc = DateTime.UtcNow;

            List<(ProjectSnapshot Snapshot, ProjectHealth Health)> projects = state.GetProjects()
                .Select(p => (p, HealthEvaluator.Evaluate(p, nowUtc)))
                .OrderByDescending(x => x.Item2.Level)
                .ThenBy(x => x.p.Project.Name)
                .ToList();

            SummaryTilesDto tiles = BuildTiles(projects);

            return new AtAGlanceDto
            {
                SummaryLine = tiles.CriticalProjects > 0
                    ? $"{tiles.CriticalProjects} {(tiles.CriticalProjects == 1 ? "project needs" : "projects need")} attention now"
                    : tiles.WarningProjects > 0 ? "No outages, a few things to look at" : "Everything looks good",
                Tiles = tiles,
                Projects = projects.Select(x => BuildRow(x.Snapshot, x.Health)).ToList(),
                NeedsAttention = projects
                    .SelectMany(x => x.Health.Issues
                        .Where(i => i.Level >= HealthLevel.Warning)
                        .Select(i => new AttentionItemDto { ProjectName = x.Snapshot.Project.Name, ProjectSlug = x.Snapshot.Project.Slug, Issue = i }))
                    .OrderByDescending(x => x.Issue.Level)
                    .ToList(),
                RunningNow = projects
                    .SelectMany(x => (x.Snapshot.GitHub?.ActiveRuns ?? []).Select(run => new ActiveRunDto { ProjectName = x.Snapshot.Project.Name, Run = run }))
                    .OrderByDescending(x => x.Run.CreatedUtc)
                    .ToList()
            };
        }

        public ProjectDetailsDto? GetProjectDetails(string slug)
        {
            ProjectSnapshot? snapshot = state.GetProject(slug);

            if (snapshot is null)
            {
                return null;
            }

            ProjectHealth health = HealthEvaluator.Evaluate(snapshot, DateTime.UtcNow);

            return new ProjectDetailsDto
            {
                Slug = snapshot.Project.Slug,
                Name = snapshot.Project.Name,
                Description = snapshot.Project.Description,
                Health = health,
                WarningCount = health.Issues.Count(x => x.Level >= HealthLevel.Warning),
                Endpoints = snapshot.Endpoints,
                GitHub = snapshot.GitHub,
                GitHubError = snapshot.GitHubError,
                Checks = snapshot.Checks,
                LatestValues = snapshot.LatestValues,
                LastReportUtc = snapshot.LastReportUtc
            };
        }

        public List<NavProjectDto> GetNavProjects()
        {
            DateTime nowUtc = DateTime.UtcNow;

            return state.GetProjects()
                .Select(p =>
                {
                    ProjectHealth health = HealthEvaluator.Evaluate(p, nowUtc);
                    return new NavProjectDto { Slug = p.Project.Slug, Name = p.Project.Name, Level = health.Level, HasData = health.HasData };
                })
                .ToList();
        }

        public DataFreshnessDto GetDataFreshness()
        {
            return new DataFreshnessDto
            {
                LastGitHubPollUtc = state.LastGitHubPollUtc,
                LastUptimeCheckUtc = state.LastUptimeCheckUtc
            };
        }

        private static SummaryTilesDto BuildTiles(List<(ProjectSnapshot Snapshot, ProjectHealth Health)> projects)
        {
            List<EndpointStatus> endpoints = projects.SelectMany(x => x.Snapshot.Endpoints).ToList();
            List<GitHubSnapshot> repos = projects.Select(x => x.Snapshot.GitHub).OfType<GitHubSnapshot>().ToList();
            List<SecurityAlertInfo> alerts = repos.SelectMany(x => x.SecurityAlerts).ToList();

            int critical = alerts.Count(x => x.Severity == "critical");
            int high = alerts.Count(x => x.Severity == "high");

            return new SummaryTilesDto
            {
                TotalProjects = projects.Count,
                HealthyProjects = projects.Count(x => x.Health.Level <= HealthLevel.Info),
                WarningProjects = projects.Count(x => x.Health.Level == HealthLevel.Warning),
                CriticalProjects = projects.Count(x => x.Health.Level == HealthLevel.Critical),
                ServicesTotal = endpoints.Count,
                ServicesUp = endpoints.Count(x => x.IsUp == true),
                ServicesDown = endpoints.Count(x => x.IsUp == false),
                AverageUptime24h = AverageUptime(endpoints),
                CriticalVulnerabilities = critical,
                HighVulnerabilities = high,
                OtherVulnerabilities = alerts.Count - critical - high,
                ReposWithoutAlertAccess = repos.Count(x => x.SecurityAlertsError is not null),
                RunningWorkflows = repos.Sum(x => x.ActiveRuns.Count),
                FailingWorkflows = repos.Sum(x => x.LatestRunPerWorkflow.Count(r => r.IsFailure)),
                UpdatePullRequests = repos.Sum(x => x.OpenPullRequests.Count(p => p.IsRenovate))
            };
        }

        private static ProjectRowDto BuildRow(ProjectSnapshot snapshot, ProjectHealth health)
        {
            GitHubSnapshot? github = snapshot.GitHub;

            return new ProjectRowDto
            {
                Slug = snapshot.Project.Slug,
                Name = snapshot.Project.Name,
                Description = snapshot.Project.Description,
                Health = health,
                Endpoints = snapshot.Endpoints,
                AverageUptime24h = AverageUptime(snapshot.Endpoints),
                HasGitHubData = github is not null,
                LatestRuns = github?.LatestRunPerWorkflow ?? [],
                LastDeploy = github?.LatestRunPerWorkflow
                    .Where(r => HealthEvaluator.IsCriticalWorkflow(r.WorkflowName) && !r.WorkflowName.Contains("renovate", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(r => r.UpdatedUtc)
                    .FirstOrDefault(),
                SecurityAlerts = github?.SecurityAlerts ?? [],
                SecurityAlertsError = github?.SecurityAlertsError,
                OpenPullRequests = github?.OpenPullRequests.Count,
                TopIssue = health.Issues.FirstOrDefault(x => x.Level >= HealthLevel.Warning)
            };
        }

        private static double? AverageUptime(IEnumerable<EndpointStatus> endpoints)
        {
            List<double> values = endpoints.Where(x => x.Uptime24h is not null).Select(x => x.Uptime24h!.Value).ToList();
            return values.Count == 0 ? null : values.Average();
        }
    }
}
