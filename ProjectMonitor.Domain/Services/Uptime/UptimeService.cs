using Hangfire;
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
        IUptimeProbeHelper probe,
        IDashboardStateHelper state,
        FailureStreakTracker streaks,
        IOptions<MonitorConfig> config) : IUptimeService
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckEndpoints()
        {
            DateTime nowUtc = DateTime.UtcNow;

            var targets = config.Value.Projects
                .SelectMany(p => p.Endpoints.Select(e => new { Project = p, Endpoint = e, Key = DashboardStateHelper.EndpointKey(p, e) }))
                .ToList();

            if (targets.Count == 0)
            {
                state.MarkUptimeCheckComplete();
                return;
            }

            ProbeResult[] results = await Task.WhenAll(targets.Select(t => probe.CheckEndpoint(t.Endpoint)));

            context.UptimeChecks.AddRange(targets.Select((t, i) => new UptimeCheck
            {
                EndpointKey = t.Key,
                CheckedAtUtc = nowUtc,
                IsUp = results[i].IsUp,
                StatusCode = results[i].StatusCode,
                ResponseTimeMs = results[i].ResponseTimeMs,
                Error = results[i].Error is { Length: > 500 } error ? error[..500] : results[i].Error
            }));
            await context.SaveChangesAsync();

            Dictionary<string, UptimeStats> stats = await UptimeStatsHelper.GetStats(context, targets.Select(t => t.Key).ToList(), nowUtc);

            foreach (var project in targets.Select((t, i) => (Target: t, Result: results[i])).GroupBy(x => x.Target.Project.Slug))
            {
                IReadOnlyList<EndpointStatus> previous = state.GetProject(project.Key)?.Endpoints ?? [];

                List<EndpointStatus> endpoints = project.Select(x =>
                {
                    (bool isDown, DateTime? downSince) = streaks.Record(x.Target.Key, x.Result.IsUp, nowUtc);
                    UptimeStats endpointStats = stats[x.Target.Key];
                    bool? wasUp = previous.FirstOrDefault(e => e.Key == x.Target.Key)?.IsUp;

                    if (isDown && wasUp != false)
                    {
                        Log.Warning("[Uptime] {Endpoint} is down: {Error}", x.Target.Key, x.Result.Error);
                    }
                    else if (!isDown && wasUp == false)
                    {
                        Log.Information("[Uptime] {Endpoint} is back up", x.Target.Key);
                    }

                    return new EndpointStatus(
                        Key: x.Target.Key,
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
                        History: endpointStats.Last24Hours);
                }).ToList();

                state.UpdateEndpoints(project.Key, endpoints);
            }

            state.MarkUptimeCheckComplete();
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task PruneOldChecks()
        {
            DateTime cutoff = DateTime.UtcNow.AddDays(-Math.Max(1, config.Value.UptimeRetentionDays));
            int deleted = await context.UptimeChecks.Where(x => x.CheckedAtUtc < cutoff).ExecuteDeleteAsync();

            Log.Information("[Uptime] Pruned {Count} checks older than {Cutoff:u}", deleted, cutoff);
        }
    }
}
