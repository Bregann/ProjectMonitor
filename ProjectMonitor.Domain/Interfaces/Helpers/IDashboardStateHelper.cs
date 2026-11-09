using ProjectMonitor.Domain.DTOs.Monitoring;

namespace ProjectMonitor.Domain.Interfaces.Helpers
{
    /// <summary>In-memory picture of every project. Jobs write to it; the dashboard reads it.</summary>
    public interface IDashboardStateHelper
    {
        event Action? Changed;
        DateTime? LastGitHubPollUtc { get; }
        DateTime? LastUptimeCheckUtc { get; }
        IReadOnlyList<ProjectSnapshot> GetProjects();
        ProjectSnapshot? GetProject(string slug);
        void UpdateGitHub(string slug, GitHubSnapshot? snapshot, string? error);
        void MarkGitHubPollComplete();
        void UpdateEndpoints(string slug, IReadOnlyList<EndpointStatus> endpoints);
        void MarkUptimeCheckComplete();
    }
}
