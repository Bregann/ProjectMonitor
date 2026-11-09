using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectMonitor.Domain.Configuration;
using ProjectMonitor.Domain.State;

namespace ProjectMonitor.Domain.GitHub
{
    public class GitHubPollingService(
        IServiceScopeFactory scopeFactory,
        DashboardState state,
        IOptionsMonitor<MonitorOptions> options,
        ILogger<GitHubPollingService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new(TimeSpan.FromSeconds(Math.Max(30, options.CurrentValue.GitHubPollSeconds)));

            do
            {
                await PollAllAsync(stoppingToken);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        public async Task PollAllAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = scopeFactory.CreateScope();
            IGitHubClient client = scope.ServiceProvider.GetRequiredService<IGitHubClient>();

            IEnumerable<Task> polls = options.CurrentValue.Projects
                .Where(x => !string.IsNullOrWhiteSpace(x.Repository))
                .Select(project => PollProjectAsync(client, project, cancellationToken));

            await Task.WhenAll(polls);
            state.MarkGitHubPollComplete();
        }

        private async Task PollProjectAsync(IGitHubClient client, ProjectOptions project, CancellationToken cancellationToken)
        {
            string repo = project.Repository!;

            try
            {
                Task<GitHubRepository> repository = client.GetRepositoryAsync(repo, cancellationToken);
                Task<List<GitHubWorkflowRun>> runs = client.GetWorkflowRunsAsync(repo, cancellationToken);
                Task<List<GitHubPullRequest>> pullRequests = client.GetOpenPullRequestsAsync(repo, cancellationToken);
                Task<List<GitHubIssue>> issues = client.GetOpenIssuesAsync(repo, cancellationToken);
                Task<List<GitHubDependabotAlert>?> alerts = client.GetOpenDependabotAlertsAsync(repo, cancellationToken);

                await Task.WhenAll(repository, runs, pullRequests, issues, alerts);

                GitHubSnapshot snapshot = GitHubSnapshotBuilder.Build(repository.Result, runs.Result, pullRequests.Result, issues.Result, alerts.Result, DateTime.UtcNow);
                state.UpdateGitHub(project.Slug, snapshot, null);
            }
            catch (Exception ex) when (ex is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(ex, "GitHub poll failed for {Repository}", repo);
                state.UpdateGitHub(project.Slug, null, $"Couldn't load GitHub data: {ex.Message}");
            }
        }
    }
}
