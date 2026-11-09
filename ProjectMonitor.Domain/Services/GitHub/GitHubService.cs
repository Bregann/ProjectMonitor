using ProjectMonitor.Domain.DTOs.Configuration;
using ProjectMonitor.Domain.DTOs.GitHub;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Helpers;
using ProjectMonitor.Domain.Interfaces.Helpers;
using ProjectMonitor.Domain.Interfaces.Jobs;
using Serilog;

namespace ProjectMonitor.Domain.Services.GitHub
{
    public class GitHubService(IGitHubApiHelper gitHubApi, IProjectConfigHelper projectConfigHelper, IDashboardStateHelper state) : IGitHubService
    {
        public async Task PollRepositories()
        {
            List<ProjectConfig> projects = await projectConfigHelper.SyncDashboardState();

            IEnumerable<Task> polls = projects
                .Where(x => !string.IsNullOrWhiteSpace(x.Repository))
                .Select(PollProject);

            await Task.WhenAll(polls);
            state.MarkGitHubPollComplete();
        }

        private async Task PollProject(ProjectConfig project)
        {
            string repo = project.Repository!;

            try
            {
                Task<GitHubRepositoryResponse> repository = gitHubApi.GetRepository(repo);
                Task<List<GitHubWorkflowRun>> runs = gitHubApi.GetWorkflowRuns(repo);
                Task<List<GitHubPullRequestResponse>> pullRequests = gitHubApi.GetOpenPullRequests(repo);
                Task<List<GitHubIssueResponse>> issues = gitHubApi.GetOpenIssues(repo);
                Task<List<GitHubDependabotAlertResponse>?> alerts = gitHubApi.GetOpenDependabotAlerts(repo);

                await Task.WhenAll(repository, runs, pullRequests, issues, alerts);

                GitHubSnapshot snapshot = GitHubSnapshotBuilder.Build(repository.Result, runs.Result, pullRequests.Result, issues.Result, alerts.Result, DateTime.UtcNow);
                state.UpdateGitHub(project.Slug, snapshot, null);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "[GitHub] Poll failed for {Repository}", repo);
                state.UpdateGitHub(project.Slug, null, $"Couldn't load GitHub data: {ex.Message}");
            }
        }
    }
}
