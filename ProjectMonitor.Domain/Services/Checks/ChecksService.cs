using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectMonitor.Domain.Database.Context;
using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.DTOs.Configuration;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Interfaces.Helpers;
using ProjectMonitor.Domain.Interfaces.Jobs;
using Serilog;

namespace ProjectMonitor.Domain.Services.Checks
{
    public class ChecksService(
        AppDbContext context,
        IEnumerable<ICheckEvaluator> evaluators,
        IProjectConfigHelper projectConfigHelper,
        IDashboardStateHelper state,
        IOptions<MonitorConfig> config) : IChecksService
    {
        public async Task EvaluateAll()
        {
            List<ProjectConfig> projects = await projectConfigHelper.SyncDashboardState();

            foreach (ProjectConfig project in projects)
            {
                await Evaluate(project);
            }
        }

        public async Task EvaluateProject(int projectId)
        {
            List<ProjectConfig> projects = await projectConfigHelper.SyncDashboardState();
            ProjectConfig? project = projects.FirstOrDefault(x => x.Id == projectId);

            if (project is not null)
            {
                await Evaluate(project);
            }
        }

        public async Task PruneReportedValues()
        {
            DateTime cutoff = DateTime.UtcNow.AddDays(-Math.Max(1, config.Value.ReportedValueRetentionDays));
            int deleted = await context.ReportedValues.Where(x => x.ReportedAtUtc < cutoff).ExecuteDeleteAsync();

            Log.Information("[Checks] Pruned {Count} reported values older than {Cutoff:u}", deleted, cutoff);
        }

        private async Task Evaluate(ProjectConfig project)
        {
            DateTime nowUtc = DateTime.UtcNow;

            // HTTP checks are run by the uptime job; everything else is evaluated here
            List<ProjectCheck> checks = await context.ProjectChecks
                .AsNoTracking()
                .Where(x => x.ProjectId == project.Id && x.IsEnabled && !(x is HttpCheck))
                .OrderBy(x => x.Name)
                .ToListAsync();

            // Latest value of each key: the row whose time is the newest for that key
            var latestRows = await context.ReportedValues
                .AsNoTracking()
                .Where(x => x.ProjectId == project.Id
                    && x.ReportedAtUtc == context.ReportedValues
                        .Where(y => y.ProjectId == project.Id && y.Key == x.Key)
                        .Max(y => y.ReportedAtUtc))
                .Select(x => new { x.Id, x.Key, x.Value, x.ReportedAtUtc })
                .ToListAsync();

            List<ReportedValueInfo> latestValues = latestRows
                .GroupBy(x => x.Key)
                .Select(g => g.OrderByDescending(x => x.Id).First())
                .Select(x => new ReportedValueInfo(x.Key, x.Value, x.ReportedAtUtc))
                .ToList();

            DateTime? lastReportUtc = latestValues.Count == 0 ? null : latestValues.Max(x => x.ReportedAtUtc);
            CheckEvaluationContext evaluationContext = new(latestValues.ToDictionary(x => x.Key), lastReportUtc, nowUtc);

            List<CheckStatus> statuses = checks.Select(check =>
            {
                ICheckEvaluator? evaluator = evaluators.FirstOrDefault(e => e.CheckType == check.CheckType);

                return evaluator is null
                    ? new CheckStatus(check.Id, check.Name, check.CheckType, HealthLevel.Warning, check.Name, $"No evaluator registered for {check.CheckType} checks", null, null)
                    : evaluator.Evaluate(check, evaluationContext);
            }).ToList();

            state.UpdateChecks(project.Slug, statuses, latestValues.OrderBy(x => x.Key).ToList(), lastReportUtc);
        }
    }
}
