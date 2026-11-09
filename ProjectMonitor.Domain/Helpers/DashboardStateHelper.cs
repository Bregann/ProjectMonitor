using ProjectMonitor.Domain.DTOs.Configuration;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Interfaces.Helpers;

namespace ProjectMonitor.Domain.Helpers
{
    public class DashboardStateHelper : IDashboardStateHelper
    {
        private readonly Lock _lock = new();
        private Dictionary<string, ProjectSnapshot> _projects = [];
        private List<string> _order = [];

        public event Action? Changed;

        public DateTime? LastGitHubPollUtc { get; private set; }

        public DateTime? LastUptimeCheckUtc { get; private set; }

        public IReadOnlyList<ProjectSnapshot> GetProjects()
        {
            lock (_lock)
            {
                return _order.Select(x => _projects[x]).ToList();
            }
        }

        public ProjectSnapshot? GetProject(string slug)
        {
            lock (_lock)
            {
                return _projects.GetValueOrDefault(slug);
            }
        }

        public void SyncProjects(IReadOnlyList<ProjectConfig> projects)
        {
            lock (_lock)
            {
                Dictionary<string, ProjectSnapshot> updated = [];

                foreach (ProjectConfig project in projects)
                {
                    ProjectSnapshot? existing = _projects.GetValueOrDefault(project.Slug);

                    // Keep known uptime results for endpoints that still exist; add blank ones for new endpoints
                    List<EndpointStatus> endpoints = project.Endpoints
                        .Select(e =>
                        {
                            EndpointStatus? known = existing?.Endpoints.FirstOrDefault(x => x.Key == EndpointKey(e));
                            return known is null
                                ? NewEndpointStatus(e)
                                : known with { Name = e.Name, Url = e.Url, FailureLevel = e.FailureLevel };
                        })
                        .ToList();

                    // A repo change makes the old GitHub data meaningless
                    bool sameRepo = existing?.Project.Repository == project.Repository;

                    updated[project.Slug] = existing is null
                        ? new ProjectSnapshot(project, endpoints, null, null, null)
                        : existing with
                        {
                            Project = project,
                            Endpoints = endpoints,
                            GitHub = sameRepo ? existing.GitHub : null,
                            GitHubUpdatedUtc = sameRepo ? existing.GitHubUpdatedUtc : null,
                            GitHubError = sameRepo ? existing.GitHubError : null
                        };
                }

                _projects = updated;
                _order = projects.Select(x => x.Slug).ToList();
            }

            Changed?.Invoke();
        }

        public void UpdateGitHub(string slug, GitHubSnapshot? snapshot, string? error)
        {
            lock (_lock)
            {
                if (!_projects.TryGetValue(slug, out ProjectSnapshot? current))
                {
                    return;
                }

                // Keep the last good data when a poll fails, so a GitHub blip doesn't blank the page
                _projects[slug] = error is null
                    ? current with { GitHub = snapshot, GitHubUpdatedUtc = DateTime.UtcNow, GitHubError = null }
                    : current with { GitHubError = error };
            }
        }

        public void MarkGitHubPollComplete()
        {
            LastGitHubPollUtc = DateTime.UtcNow;
            Changed?.Invoke();
        }

        public void UpdateEndpoints(string slug, IReadOnlyList<EndpointStatus> endpoints)
        {
            lock (_lock)
            {
                if (_projects.TryGetValue(slug, out ProjectSnapshot? current))
                {
                    _projects[slug] = current with { Endpoints = endpoints };
                }
            }
        }

        public void MarkUptimeCheckComplete()
        {
            LastUptimeCheckUtc = DateTime.UtcNow;
            Changed?.Invoke();
        }

        public void UpdateChecks(string slug, IReadOnlyList<CheckStatus> checks, IReadOnlyList<ReportedValueInfo> latestValues, DateTime? lastReportUtc)
        {
            lock (_lock)
            {
                if (_projects.TryGetValue(slug, out ProjectSnapshot? current))
                {
                    _projects[slug] = current with { Checks = checks, LatestValues = latestValues, LastReportUtc = lastReportUtc };
                }
            }

            Changed?.Invoke();
        }

        public static string EndpointKey(EndpointConfig endpoint)
        {
            return endpoint.Id.ToString();
        }

        private static EndpointStatus NewEndpointStatus(EndpointConfig endpoint)
        {
            return new EndpointStatus(EndpointKey(endpoint), endpoint.Name, endpoint.Url, null, null, null, null, null, null, null, null, []) { FailureLevel = endpoint.FailureLevel };
        }
    }
}
