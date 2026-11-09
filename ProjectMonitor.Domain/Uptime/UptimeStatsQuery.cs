using Microsoft.EntityFrameworkCore;
using ProjectMonitor.Domain.Data;
using ProjectMonitor.Domain.State;

namespace ProjectMonitor.Domain.Uptime
{
    public record UptimeStats(double? Uptime24h, double? Uptime7d, IReadOnlyList<UptimeBucket> Last24Hours);

    public static class UptimeStatsQuery
    {
        /// <summary>
        /// Uptime percentages and an hourly history strip per endpoint. Aggregation happens in the
        /// database (GROUP BY), so a week of minute-by-minute checks never gets loaded into memory.
        /// </summary>
        public static async Task<Dictionary<string, UptimeStats>> GetAsync(MonitorDbContext db, IReadOnlyCollection<string> keys, DateTime nowUtc, CancellationToken cancellationToken)
        {
            DateTime weekAgo = nowUtc.AddDays(-7);
            DateTime currentHour = new(nowUtc.Year, nowUtc.Month, nowUtc.Day, nowUtc.Hour, 0, 0, DateTimeKind.Utc);
            DateTime dayStart = currentHour.AddHours(-23);

            var daily = await db.UptimeChecks
                .Where(x => keys.Contains(x.EndpointKey) && x.CheckedAtUtc >= weekAgo)
                .GroupBy(x => x.EndpointKey)
                .Select(g => new { Key = g.Key, Total = g.Count(), Up = g.Count(x => x.IsUp) })
                .ToListAsync(cancellationToken);

            var hourly = await db.UptimeChecks
                .Where(x => keys.Contains(x.EndpointKey) && x.CheckedAtUtc >= dayStart)
                .GroupBy(x => new { x.EndpointKey, x.CheckedAtUtc.Date, x.CheckedAtUtc.Hour })
                .Select(g => new { g.Key.EndpointKey, g.Key.Date, g.Key.Hour, Total = g.Count(), Up = g.Count(x => x.IsUp) })
                .ToListAsync(cancellationToken);

            Dictionary<string, UptimeStats> result = [];

            foreach (string key in keys)
            {
                var week = daily.FirstOrDefault(x => x.Key == key);
                var hours = hourly.Where(x => x.EndpointKey == key).ToList();

                List<UptimeBucket> buckets = Enumerable.Range(0, 24)
                    .Select(i => dayStart.AddHours(i))
                    .Select(start =>
                    {
                        var bucket = hours.FirstOrDefault(h => h.Date == start.Date && h.Hour == start.Hour);
                        return new UptimeBucket(start, bucket is null ? null : Percent(bucket.Up, bucket.Total));
                    })
                    .ToList();

                int dayTotal = hours.Sum(x => x.Total);
                int dayUp = hours.Sum(x => x.Up);

                result[key] = new UptimeStats(
                    Uptime24h: dayTotal == 0 ? null : Percent(dayUp, dayTotal),
                    Uptime7d: week is null ? null : Percent(week.Up, week.Total),
                    Last24Hours: buckets);
            }

            return result;
        }

        private static double Percent(int up, int total)
        {
            return total == 0 ? 0 : Math.Round(100.0 * up / total, 2);
        }
    }
}
