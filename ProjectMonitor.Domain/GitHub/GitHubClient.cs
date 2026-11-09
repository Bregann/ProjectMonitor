using Microsoft.Extensions.Options;
using ProjectMonitor.Domain.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProjectMonitor.Domain.GitHub
{
    public interface IGitHubClient
    {
        Task<GitHubRepository> GetRepositoryAsync(string repo, CancellationToken cancellationToken);

        Task<List<GitHubWorkflowRun>> GetWorkflowRunsAsync(string repo, CancellationToken cancellationToken);

        Task<List<GitHubPullRequest>> GetOpenPullRequestsAsync(string repo, CancellationToken cancellationToken);

        Task<List<GitHubIssue>> GetOpenIssuesAsync(string repo, CancellationToken cancellationToken);

        /// <summary>Returns null when the token can't read Dependabot alerts (or they are turned off for the repo).</summary>
        Task<List<GitHubDependabotAlert>?> GetOpenDependabotAlertsAsync(string repo, CancellationToken cancellationToken);
    }

    public class GitHubClient(HttpClient httpClient, IOptionsMonitor<MonitorOptions> options) : IGitHubClient
    {
        public Task<GitHubRepository> GetRepositoryAsync(string repo, CancellationToken cancellationToken)
        {
            return GetAsync<GitHubRepository>($"repos/{Owner}/{repo}", cancellationToken);
        }

        public async Task<List<GitHubWorkflowRun>> GetWorkflowRunsAsync(string repo, CancellationToken cancellationToken)
        {
            GitHubWorkflowRunList list = await GetAsync<GitHubWorkflowRunList>($"repos/{Owner}/{repo}/actions/runs?per_page=50", cancellationToken);
            return list.WorkflowRuns;
        }

        public Task<List<GitHubPullRequest>> GetOpenPullRequestsAsync(string repo, CancellationToken cancellationToken)
        {
            return GetAsync<List<GitHubPullRequest>>($"repos/{Owner}/{repo}/pulls?state=open&per_page=50", cancellationToken);
        }

        public async Task<List<GitHubIssue>> GetOpenIssuesAsync(string repo, CancellationToken cancellationToken)
        {
            List<GitHubIssue> issues = await GetAsync<List<GitHubIssue>>($"repos/{Owner}/{repo}/issues?state=open&per_page=100", cancellationToken);

            // The issues API also returns pull requests
            return issues.Where(x => x.PullRequest is null).ToList();
        }

        public async Task<List<GitHubDependabotAlert>?> GetOpenDependabotAlertsAsync(string repo, CancellationToken cancellationToken)
        {
            using HttpRequestMessage request = CreateRequest($"repos/{Owner}/{repo}/dependabot/alerts?state=open&per_page=100");
            using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<GitHubDependabotAlert>>(cancellationToken) ?? [];
        }

        private string Owner => options.CurrentValue.GitHubOwner;

        private async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken)
        {
            using HttpRequestMessage request = CreateRequest(path);
            using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
                ?? throw new InvalidOperationException($"GitHub returned an empty response for {path}");
        }

        private HttpRequestMessage CreateRequest(string path)
        {
            HttpRequestMessage request = new(HttpMethod.Get, path);
            string token = options.CurrentValue.GitHubToken;

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return request;
        }

        public static void Configure(HttpClient client)
        {
            client.BaseAddress = new Uri("https://api.github.com/");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("ProjectMonitor");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.Timeout = TimeSpan.FromSeconds(30);
        }
    }
}
