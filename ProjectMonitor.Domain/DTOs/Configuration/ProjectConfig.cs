namespace ProjectMonitor.Domain.DTOs.Configuration
{
    public class ProjectConfig
    {
        /// <summary>URL-safe identifier, used for the project's page (/projects/{slug}).</summary>
        public string Slug { get; set; } = "";

        public string Name { get; set; } = "";

        public string? Description { get; set; }

        /// <summary>Repository name under <see cref="MonitorConfig.GitHubOwner"/>. Leave empty for projects without a repo.</summary>
        public string? Repository { get; set; }

        public List<EndpointConfig> Endpoints { get; set; } = [];
    }
}
