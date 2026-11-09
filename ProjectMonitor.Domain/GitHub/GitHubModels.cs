using System.Text.Json.Serialization;

namespace ProjectMonitor.Domain.GitHub
{
    // Only the fields the dashboard uses. Property names follow GitHub's snake_case JSON.

    public record GitHubRepository(
        [property: JsonPropertyName("html_url")] string HtmlUrl,
        [property: JsonPropertyName("default_branch")] string DefaultBranch);

    public record GitHubWorkflowRunList(
        [property: JsonPropertyName("workflow_runs")] List<GitHubWorkflowRun> WorkflowRuns);

    public record GitHubWorkflowRun(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("workflow_id")] long WorkflowId,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("display_title")] string? DisplayTitle,
        [property: JsonPropertyName("head_branch")] string? HeadBranch,
        [property: JsonPropertyName("event")] string Event,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("conclusion")] string? Conclusion,
        [property: JsonPropertyName("html_url")] string HtmlUrl,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("updated_at")] DateTime UpdatedAt,
        [property: JsonPropertyName("actor")] GitHubUser? Actor);

    public record GitHubUser(
        [property: JsonPropertyName("login")] string Login);

    public record GitHubPullRequest(
        [property: JsonPropertyName("number")] int Number,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("html_url")] string HtmlUrl,
        [property: JsonPropertyName("user")] GitHubUser? User,
        [property: JsonPropertyName("head")] GitHubRef Head,
        [property: JsonPropertyName("draft")] bool Draft,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt);

    public record GitHubRef(
        [property: JsonPropertyName("ref")] string Ref);

    public record GitHubIssue(
        [property: JsonPropertyName("number")] int Number,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("html_url")] string HtmlUrl,
        [property: JsonPropertyName("updated_at")] DateTime UpdatedAt,
        [property: JsonPropertyName("labels")] List<GitHubLabel>? Labels,
        [property: JsonPropertyName("pull_request")] object? PullRequest);

    public record GitHubLabel(
        [property: JsonPropertyName("name")] string Name);

    public record GitHubDependabotAlert(
        [property: JsonPropertyName("number")] int Number,
        [property: JsonPropertyName("html_url")] string HtmlUrl,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("dependency")] GitHubAlertDependency? Dependency,
        [property: JsonPropertyName("security_advisory")] GitHubSecurityAdvisory? SecurityAdvisory,
        [property: JsonPropertyName("security_vulnerability")] GitHubSecurityVulnerability? SecurityVulnerability);

    public record GitHubAlertDependency(
        [property: JsonPropertyName("manifest_path")] string? ManifestPath,
        [property: JsonPropertyName("relationship")] string? Relationship);

    public record GitHubSecurityAdvisory(
        [property: JsonPropertyName("summary")] string? Summary,
        [property: JsonPropertyName("severity")] string? Severity);

    public record GitHubSecurityVulnerability(
        [property: JsonPropertyName("package")] GitHubPackage? Package,
        [property: JsonPropertyName("severity")] string? Severity,
        [property: JsonPropertyName("first_patched_version")] GitHubPatchedVersion? FirstPatchedVersion);

    public record GitHubPackage(
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("ecosystem")] string? Ecosystem);

    public record GitHubPatchedVersion(
        [property: JsonPropertyName("identifier")] string? Identifier);
}
