using System.Text.Json.Serialization;

namespace ProjectMonitor.Domain.DTOs.GitHub
{
    public class GitHubDependabotAlertResponse
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = "";

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("dependency")]
        public GitHubAlertDependency? Dependency { get; set; }

        [JsonPropertyName("security_advisory")]
        public GitHubSecurityAdvisory? SecurityAdvisory { get; set; }

        [JsonPropertyName("security_vulnerability")]
        public GitHubSecurityVulnerability? SecurityVulnerability { get; set; }
    }

    public class GitHubAlertDependency
    {
        [JsonPropertyName("manifest_path")]
        public string? ManifestPath { get; set; }

        [JsonPropertyName("relationship")]
        public string? Relationship { get; set; }
    }

    public class GitHubSecurityAdvisory
    {
        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("severity")]
        public string? Severity { get; set; }
    }

    public class GitHubSecurityVulnerability
    {
        [JsonPropertyName("package")]
        public GitHubPackage? Package { get; set; }

        [JsonPropertyName("severity")]
        public string? Severity { get; set; }

        [JsonPropertyName("first_patched_version")]
        public GitHubPatchedVersion? FirstPatchedVersion { get; set; }
    }

    public class GitHubPackage
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("ecosystem")]
        public string? Ecosystem { get; set; }
    }

    public class GitHubPatchedVersion
    {
        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }
    }
}
