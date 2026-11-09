using System.Text.Json.Serialization;

namespace ProjectMonitor.Domain.DTOs.GitHub
{
    public class GitHubPullRequestResponse
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = "";

        [JsonPropertyName("user")]
        public GitHubUser? User { get; set; }

        [JsonPropertyName("head")]
        public GitHubRef Head { get; set; } = new();

        [JsonPropertyName("draft")]
        public bool Draft { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class GitHubRef
    {
        [JsonPropertyName("ref")]
        public string Ref { get; set; } = "";
    }
}
