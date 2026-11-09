using System.Text.Json.Serialization;

namespace ProjectMonitor.Domain.DTOs.GitHub
{
    public class GitHubIssueResponse
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = "";

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("labels")]
        public List<GitHubLabel> Labels { get; set; } = [];

        /// <summary>Set when the "issue" is actually a pull request (the issues API returns both).</summary>
        [JsonPropertyName("pull_request")]
        public object? PullRequest { get; set; }
    }

    public class GitHubLabel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
}
