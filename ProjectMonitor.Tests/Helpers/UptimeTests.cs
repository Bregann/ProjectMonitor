using Microsoft.EntityFrameworkCore;
using ProjectMonitor.Domain.Database.Context;
using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Helpers;

namespace ProjectMonitor.Tests.Helpers
{
    public class UptimeTests
    {
        [TestCase(200, true)]
        [TestCase(401, true)]
        [TestCase(404, true)]
        [TestCase(500, false)]
        [TestCase(503, false)]
        public void IsUpStatus_WithoutExpectedCodes_AnythingBelow500IsUp(int status, bool expected)
        {
            Assert.That(UptimeProbeHelper.IsUpStatus(status, []), Is.EqualTo(expected));
        }

        [Test]
        public void IsUpStatus_WithExpectedCodes_OnlyThoseCount()
        {
            Assert.That(UptimeProbeHelper.IsUpStatus(200, [200]), Is.True);
            Assert.That(UptimeProbeHelper.IsUpStatus(404, [200]), Is.False);
        }

        [Test]
        public void FailureStreakTracker_OnlyDownAfterConsecutiveFailures()
        {
            FailureStreakTracker tracker = new(2);
            DateTime t0 = TestData.Now;

            Assert.That(tracker.Record("a", false, t0).IsDown, Is.False, "one blip is not an outage");

            (bool isDown, DateTime? since) = tracker.Record("a", false, t0.AddMinutes(1));
            Assert.That(isDown, Is.True);
            Assert.That(since, Is.EqualTo(t0), "down since the first failure in the streak");

            Assert.That(tracker.Record("a", true, t0.AddMinutes(2)).IsDown, Is.False);
            Assert.That(tracker.Record("a", false, t0.AddMinutes(3)).IsDown, Is.False, "a success resets the streak");
        }

        [Test]
        public async Task UptimeStatsHelper_CalculatesPercentagesAndHourlyBuckets()
        {
            DbContextOptions<AppDbContext> dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            await using AppDbContext db = new(dbOptions);

            DateTime now = TestData.Now.AddMinutes(30); // 12:30

            // Last hour: 3 up, 1 down. Two days ago: 4 up (counts towards 7d only).
            db.UptimeChecks.AddRange(
                Check(1, now.AddMinutes(-1), true),
                Check(1, now.AddMinutes(-2), true),
                Check(1, now.AddMinutes(-3), true),
                Check(1, now.AddMinutes(-4), false),
                Check(1, now.AddDays(-2), true),
                Check(1, now.AddDays(-2).AddMinutes(1), true),
                Check(1, now.AddDays(-2).AddMinutes(2), true),
                Check(1, now.AddDays(-2).AddMinutes(3), true),
                Check(1, now.AddDays(-10), false),
                Check(2, now.AddMinutes(-1), false));
            await db.SaveChangesAsync();

            Dictionary<int, UptimeStats> stats = await UptimeStatsHelper.GetStats(db, [1], now);
            UptimeStats api = stats[1];

            Assert.That(api.Uptime24h, Is.EqualTo(75.0));
            Assert.That(api.Uptime7d, Is.EqualTo(87.5));
            Assert.That(api.Last24Hours, Has.Count.EqualTo(24));
            Assert.That(api.Last24Hours[^1].StartUtc, Is.EqualTo(TestData.Now), "the last bucket is the current hour (12:00)");
            Assert.That(api.Last24Hours[^1].UptimePercent, Is.EqualTo(75.0), "12:26-12:29 has 3 up and 1 down");
            Assert.That(api.Last24Hours[^2].UptimePercent, Is.Null, "no checks between 11:00 and 12:00");
            Assert.That(api.Last24Hours[0].UptimePercent, Is.Null, "no checks in that hour");
        }

        private static UptimeCheck Check(int httpCheckId, DateTime at, bool up)
        {
            return new UptimeCheck { HttpCheckId = httpCheckId, CheckedAtUtc = at, IsUp = up, StatusCode = up ? 200 : 503 };
        }
    }
}
