using Microsoft.Extensions.Options;
using ProjectMonitor.Domain.Configuration;

namespace ProjectMonitor.Domain.State
{
    /// <summary>
    /// In-memory picture of every project. Background services write to it; Blazor pages read it
    /// and subscribe to <see cref="Changed"/> to refresh live.
    /// </summary>
    public class DashboardState(IOptions<MonitorOptions> options)
    {
        private readonly object _lock = new();

        private readonly Dictionary<string, ProjectSnapshot> _projects = options.Value.Projects.ToDictionary(
            x => x.Slug,
            x => new ProjectSnapshot(x, x.Endpoints.Select(e => NewEndpointStatus(x, e)).ToList(), null, null, null));

        private readonly List<string> _order = options.Value.Projects.Select(x => x.Slug).ToList();

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

        public static string EndpointKey(ProjectOptions project, EndpointOptions endpoint)
        {
            return $"{project.Slug}/{endpoint.Name}";
        }

        private static EndpointStatus NewEndpointStatus(ProjectOptions project, EndpointOptions endpoint)
        {
            return new EndpointStatus(EndpointKey(project, endpoint), endpoint.Name, endpoint.Url, null, null, null, null, null, null, null, null, []);
        }
    }
}
