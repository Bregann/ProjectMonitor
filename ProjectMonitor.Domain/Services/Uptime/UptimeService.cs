using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectMonitor.Domain.Database.Context;
using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.DTOs.Configuration;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Helpers;
using ProjectMonitor.Domain.Interfaces.Helpers;
using ProjectMonitor.Domain.Interfaces.Jobs;
using Serilog;

namespace ProjectMonitor.Domain.Services.Uptime
{
    public class UptimeService(
        AppDbContext context,
        IProjectConfigHelper projectConfigHelper,
        IUptimeProbeHelper probe,
        IDashboardStateHelper state,
        FailureStreakTracker streaks,
        IOptions<MonitorConfig> config) : IUptimeService
    {
        public async Task CheckEndpoints()
        {
            DateTime nowUtc = DateTime.UtcNow;
            List<ProjectConfig> projects = await projectConfigHelper.SyncDashboardState();

            var targets = projects
                .SelectMany(p => p.Endpoints.Select(e => new { Project = p, Endpoint = e }))
                .ToList();

            if (targets.Count == 0)
            {
                state.MarkUptimeCheckComplete();
                return;
            }

            ProbeResult[] results = await Task.WhenAll(targets.Select(t => probe.CheckEndpoint(t.Endpoint)));

            context.UptimeChecks.AddRange(targets.Select((t, i) => new UptimeCheck
            {
                HttpCheckId = t.Endpoint.Id,
                CheckedAtUtc = nowUtc,
                IsUp = results[i].IsUp,
                StatusCode = results[i].StatusCode,
                ResponseTimeMs = results[i].ResponseTimeMs,
                Error = results[i].Error is { Length: > 500 } error ? error[..500] : results[i].Error
            }));
            await context.SaveChangesAsync();

            Dictionary<int, UptimeStats> stats = await UptimeStatsHelper.GetStats(context, targets.Select(t => t.Endpoint.Id).ToList(), nowUtc);

            foreach (var project in targets.Select((t, i) => (Target: t, Result: results[i])).GroupBy(x => x.Target.Project.Slug))
            {
                IReadOnlyList<EndpointStatus> previous = state.GetProject(project.Key)?.Endpoints ?? [];

                List<EndpointStatus> endpoints = project.Select(x =>
                {
                    string key = DashboardStateHelper.EndpointKey(x.Target.Endpoint);
                    (bool isDown, DateTime? downSince) = streaks.Record(key, x.Result.IsUp, nowUtc);
                    UptimeStats endpointStats = stats[x.Target.Endpoint.Id];
                    bool? wasUp = previous.FirstOrDefault(e => e.Key == key)?.IsUp;

                    if (isDown && wasUp != false)
                    {
                        Log.Warning("[Uptime] {Project} {Endpoint} is down: {Error}", x.Target.Project.Name, x.Target.Endpoint.Name, x.Result.Error);
                    }
                    else if (!isDown && wasUp == false)
                    {
                        Log.Information("[Uptime] {Project} {Endpoint} is back up", x.Target.Project.Name, x.Target.Endpoint.Name);
                    }

                    return new EndpointStatus(
                        Key: key,
                        Name: x.Target.Endpoint.Name,
                        Url: x.Target.Endpoint.Url,
                        IsUp: !isDown,
                        LastCheckedUtc: nowUtc,
                        StatusCode: x.Result.StatusCode,
                        ResponseTimeMs: x.Result.ResponseTimeMs,
                        Error: x.Result.Error,
                        Uptime24h: endpointStats.Uptime24h,
                        Uptime7d: endpointStats.Uptime7d,
                        DownSinceUtc: downSince,
                        History: endpointStats.Last24Hours)
                    {
                        FailureLevel = x.Target.Endpoint.FailureLevel
                    };
                }).ToList();

                state.UpdateEndpoints(project.Key, endpoints);
            }

            state.MarkUptimeCheckComplete();
        }

        public async Task PruneOldChecks()
        {
            DateTime cutoff = DateTime.UtcNow.AddDays(-Math.Max(1, config.Value.UptimeRetentionDays));
            int deleted = await context.UptimeChecks.Where(x => x.CheckedAtUtc < cutoff).ExecuteDeleteAsync();

            Log.Information("[Uptime] Pruned {Count} checks older than {Cutoff:u}", deleted, cutoff);
        }
    }
}
