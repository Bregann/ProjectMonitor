using System.Text.Json.Serialization;

namespace ProjectMonitor.Domain.DTOs.GitHub
{
    public class GitHubRepositoryResponse
    {
        [JsonPropertyName("html_url")]
        public required string HtmlUrl { get; set; }

        [JsonPropertyName("default_branch")]
        public required string DefaultBranch { get; set; }
    }
}
