using ProjectMonitor.Domain.DTOs.GitHub;

namespace ProjectMonitor.Domain.Interfaces.Helpers
{
    public interface IGitHubApiHelper
    {
        Task<GitHubRepositoryResponse> GetRepository(string repo);
        Task<List<GitHubWorkflowRun>> GetWorkflowRuns(string repo);
        Task<List<GitHubPullRequestResponse>> GetOpenPullRequests(string repo);
        Task<List<GitHubIssueResponse>> GetOpenIssues(string repo);

        /// <summary>Returns null when the token can't read Dependabot alerts (or they're turned off for the repo).</summary>
        Task<List<GitHubDependabotAlertResponse>?> GetOpenDependabotAlerts(string repo);
    }
}
