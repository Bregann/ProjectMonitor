using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectMonitor.Domain.Configuration;
using ProjectMonitor.Domain.Data;
using ProjectMonitor.Domain.State;

namespace ProjectMonitor.Domain.Uptime
{
    public class UptimeMonitorService(
        IServiceScopeFactory scopeFactory,
        IUptimeProbe probe,
        DashboardState state,
        IOptionsMonitor<MonitorOptions> options,
        ILogger<UptimeMonitorService> logger) : BackgroundService
    {
        private readonly FailureStreakTracker _streaks = new(options.CurrentValue.FailuresBeforeDown);
        private DateTime _lastPruneUtc = DateTime.MinValue;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new(TimeSpan.FromSeconds(Math.Max(15, options.CurrentValue.UptimeCheckSeconds)));

            do
            {
                try
                {
                    await CheckAllAsync(stoppingToken);
                }
                catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(ex, "Uptime check cycle failed");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        public async Task CheckAllAsync(CancellationToken cancellationToken)
        {
            MonitorOptions monitorOptions = options.CurrentValue;
            TimeSpan timeout = TimeSpan.FromSeconds(monitorOptions.UptimeTimeoutSeconds);
            DateTime nowUtc = DateTime.UtcNow;

            var targets = monitorOptions.Projects
                .SelectMany(p => p.Endpoints.Select(e => new { Project = p, Endpoint = e, Key = DashboardState.EndpointKey(p, e) }))
                .ToList();

            if (targets.Count == 0)
            {
                state.MarkUptimeCheckComplete();
                return;
            }

            ProbeResult[] results = await Task.WhenAll(targets.Select(t => probe.CheckAsync(t.Endpoint, timeout, cancellationToken)));

            using IServiceScope scope = scopeFactory.CreateScope();
            MonitorDbContext db = scope.ServiceProvider.GetRequiredService<MonitorDbContext>();

            db.UptimeChecks.AddRange(targets.Select((t, i) => new UptimeCheck
            {
                EndpointKey = t.Key,
                CheckedAtUtc = nowUtc,
                IsUp = results[i].IsUp,
                StatusCode = results[i].StatusCode,
                ResponseTimeMs = results[i].ResponseTimeMs,
                Error = Truncate(results[i].Error, 500),
            }));
            await db.SaveChangesAsync(cancellationToken);

            Dictionary<string, UptimeStats> stats = await UptimeStatsQuery.GetAsync(db, targets.Select(t => t.Key).ToList(), nowUtc, cancellationToken);

            foreach (var group in targets.Select((t, i) => (Target: t, Result: results[i])).GroupBy(x => x.Target.Project.Slug))
            {
                List<EndpointStatus> endpoints = group.Select(x =>
                {
                    (bool isDown, DateTime? downSince) = _streaks.Record(x.Target.Key, x.Result.IsUp, nowUtc);
                    UptimeStats endpointStats = stats[x.Target.Key];

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

                state.UpdateEndpoints(group.Key, endpoints);
            }

            state.MarkUptimeCheckComplete();
            await PruneIfDueAsync(db, monitorOptions.UptimeRetentionDays, nowUtc, cancellationToken);
        }

        private async Task PruneIfDueAsync(MonitorDbContext db, int retentionDays, DateTime nowUtc, CancellationToken cancellationToken)
        {
            if (nowUtc - _lastPruneUtc < TimeSpan.FromHours(1))
            {
                return;
            }

            _lastPruneUtc = nowUtc;
            DateTime cutoff = nowUtc.AddDays(-Math.Max(1, retentionDays));

            if (db.Database.IsRelational())
            {
                int deleted = await db.UptimeChecks.Where(x => x.CheckedAtUtc < cutoff).ExecuteDeleteAsync(cancellationToken);
                logger.LogInformation("Pruned {Count} uptime checks older than {Cutoff:u}", deleted, cutoff);
            }
            else
            {
                db.UptimeChecks.RemoveRange(db.UptimeChecks.Where(x => x.CheckedAtUtc < cutoff));
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        private static string? Truncate(string? value, int maxLength)
        {
            return value is null || value.Length <= maxLength ? value : value[..maxLength];
        }
    }
}
