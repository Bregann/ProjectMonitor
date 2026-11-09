using Microsoft.EntityFrameworkCore;
using ProjectMonitor.Domain.Database.Context;
using ProjectMonitor.Domain.DTOs.Monitoring;

namespace ProjectMonitor.Domain.Helpers
{
    public static class UptimeStatsHelper
    {
        /// <summary>
        /// Uptime percentages and an hourly history strip per endpoint. Aggregation happens in the
        /// database (GROUP BY), so a week of minute-by-minute checks never gets loaded into memory.
        /// </summary>
        public static async Task<Dictionary<int, UptimeStats>> GetStats(AppDbContext context, IReadOnlyCollection<int> httpCheckIds, DateTime nowUtc)
        {
            DateTime weekAgo = nowUtc.AddDays(-7);
            DateTime currentHour = new(nowUtc.Year, nowUtc.Month, nowUtc.Day, nowUtc.Hour, 0, 0, DateTimeKind.Utc);
            DateTime dayStart = currentHour.AddHours(-23);

            var daily = await context.UptimeChecks
                .Where(x => httpCheckIds.Contains(x.HttpCheckId) && x.CheckedAtUtc >= weekAgo)
                .GroupBy(x => x.HttpCheckId)
                .Select(g => new { Key = g.Key, Total = g.Count(), Up = g.Count(x => x.IsUp) })
                .ToListAsync();

            var hourly = await context.UptimeChecks
                .Where(x => httpCheckIds.Contains(x.HttpCheckId) && x.CheckedAtUtc >= dayStart)
                .GroupBy(x => new { x.HttpCheckId, x.CheckedAtUtc.Date, x.CheckedAtUtc.Hour })
                .Select(g => new { g.Key.HttpCheckId, g.Key.Date, g.Key.Hour, Total = g.Count(), Up = g.Count(x => x.IsUp) })
                .ToListAsync();

            Dictionary<int, UptimeStats> result = [];

            foreach (int key in httpCheckIds)
            {
                var week = daily.FirstOrDefault(x => x.Key == key);
                var hours = hourly.Where(x => x.HttpCheckId == key).ToList();

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
