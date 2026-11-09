using ProjectMonitor.Domain.DTOs.Monitoring;

namespace ProjectMonitor.Domain.DTOs.Dashboard
{
    public class ProjectDetailsDto
    {
        public required string Slug { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required ProjectHealth Health { get; set; }
        public int WarningCount { get; set; }
        public required IReadOnlyList<EndpointStatus> Endpoints { get; set; }
        public GitHubSnapshot? GitHub { get; set; }
        public string? GitHubError { get; set; }
    }
}
