using Microsoft.Extensions.Options;
using ProjectMonitor.Domain.DTOs.Configuration;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Interfaces.Helpers;

namespace ProjectMonitor.Domain.Helpers
{
    public class DashboardStateHelper(IOptions<MonitorConfig> config) : IDashboardStateHelper
    {
        private readonly Lock _lock = new();

        private readonly Dictionary<string, ProjectSnapshot> _projects = config.Value.Projects.ToDictionary(
            x => x.Slug,
            x => new ProjectSnapshot(x, x.Endpoints.Select(e => NewEndpointStatus(x, e)).ToList(), null, null, null));

        private readonly List<string> _order = config.Value.Projects.Select(x => x.Slug).ToList();

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

        public static string EndpointKey(ProjectConfig project, EndpointConfig endpoint)
        {
            return $"{project.Slug}/{endpoint.Name}";
        }

        private static EndpointStatus NewEndpointStatus(ProjectConfig project, EndpointConfig endpoint)
        {
            return new EndpointStatus(EndpointKey(project, endpoint), endpoint.Name, endpoint.Url, null, null, null, null, null, null, null, null, []);
        }
    }
}
