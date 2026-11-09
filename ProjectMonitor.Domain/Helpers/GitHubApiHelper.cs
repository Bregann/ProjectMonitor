using Microsoft.Extensions.Options;
using ProjectMonitor.Domain.DTOs.Configuration;
using ProjectMonitor.Domain.DTOs.GitHub;
using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Interfaces.Helpers;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ProjectMonitor.Domain.Helpers
{
    public class GitHubApiHelper(HttpClient httpClient, IEnvironmentalSettingHelper environmentalSettingHelper, IOptions<MonitorConfig> config) : IGitHubApiHelper
    {
        private string Owner => config.Value.GitHubOwner;

        public async Task<GitHubRepositoryResponse> GetRepository(string repo)
        {
            return await Get<GitHubRepositoryResponse>($"repos/{Owner}/{repo}");
        }

        public async Task<List<GitHubWorkflowRun>> GetWorkflowRuns(string repo)
        {
            GitHubWorkflowRunsResponse response = await Get<GitHubWorkflowRunsResponse>($"repos/{Owner}/{repo}/actions/runs?per_page=50");
            return response.WorkflowRuns;
        }

        public async Task<List<GitHubPullRequestResponse>> GetOpenPullRequests(string repo)
        {
            return await Get<List<GitHubPullRequestResponse>>($"repos/{Owner}/{repo}/pulls?state=open&per_page=50");
        }

        public async Task<List<GitHubIssueResponse>> GetOpenIssues(string repo)
        {
            List<GitHubIssueResponse> issues = await Get<List<GitHubIssueResponse>>($"repos/{Owner}/{repo}/issues?state=open&per_page=100");

            // The issues API also returns pull requests
            return issues.Where(x => x.PullRequest is null).ToList();
        }

        public async Task<List<GitHubDependabotAlertResponse>?> GetOpenDependabotAlerts(string repo)
        {
            using HttpRequestMessage request = CreateRequest($"repos/{Owner}/{repo}/dependabot/alerts?state=open&per_page=100");
            using HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<GitHubDependabotAlertResponse>>() ?? [];
        }

        private async Task<T> Get<T>(string path)
        {
            using HttpRequestMessage request = CreateRequest(path);
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>()
                ?? throw new InvalidOperationException($"GitHub returned an empty response for {path}");
        }

        private HttpRequestMessage CreateRequest(string path)
        {
            HttpRequestMessage request = new(HttpMethod.Get, path);
            string? token = environmentalSettingHelper.TryGetEnvironmentalSettingValue(EnvironmentalSettingEnum.GitHubToken);

            if (token is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return request;
        }

        public static void ConfigureHttpClient(HttpClient client)
        {
            client.BaseAddress = new Uri("https://api.github.com/");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("ProjectMonitor");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.Timeout = TimeSpan.FromSeconds(30);
        }
    }
}
