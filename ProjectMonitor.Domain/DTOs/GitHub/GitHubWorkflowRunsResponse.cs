using System.Text.Json.Serialization;

namespace ProjectMonitor.Domain.DTOs.GitHub
{
    public class GitHubWorkflowRunsResponse
    {
        [JsonPropertyName("workflow_runs")]
        public List<GitHubWorkflowRun> WorkflowRuns { get; set; } = [];
    }

    public class GitHubWorkflowRun
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("workflow_id")]
        public long WorkflowId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("display_title")]
        public string? DisplayTitle { get; set; }

        [JsonPropertyName("head_branch")]
        public string? HeadBranch { get; set; }

        [JsonPropertyName("event")]
        public string Event { get; set; } = "";

        [JsonPropertyName("status")]
        public string Status { get; set; } = "";

        [JsonPropertyName("conclusion")]
        public string? Conclusion { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = "";

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("actor")]
        public GitHubUser? Actor { get; set; }
    }
}
