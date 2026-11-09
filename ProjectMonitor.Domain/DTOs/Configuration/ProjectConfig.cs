namespace ProjectMonitor.Domain.DTOs.Configuration
{
    /// <summary>A project as the background jobs and dashboard see it, loaded from the database.</summary>
    public class ProjectConfig
    {
        public int Id { get; set; }

        /// <summary>URL-safe identifier, used for the project's page (/projects/{slug}).</summary>
        public string Slug { get; set; } = "";

        public string Name { get; set; } = "";

        public string? Description { get; set; }

        /// <summary>Repository name under <see cref="MonitorConfig.GitHubOwner"/>. Empty for projects without a repo.</summary>
        public string? Repository { get; set; }

        /// <summary>The project's enabled website/API checks.</summary>
        public List<EndpointConfig> Endpoints { get; set; } = [];
    }
}
