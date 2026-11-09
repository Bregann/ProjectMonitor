using ProjectMonitor.Domain.DTOs.Configuration;

namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    /// <summary>Everything known about one project right now. Immutable, so jobs can swap it while pages read it.</summary>
    public record ProjectSnapshot(
        ProjectConfig Project,
        IReadOnlyList<EndpointStatus> Endpoints,
        GitHubSnapshot? GitHub,
        DateTime? GitHubUpdatedUtc,
        string? GitHubError);
}
