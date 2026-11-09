using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Helpers;

namespace ProjectMonitor.Tests.Helpers
{
    public class AlertTrackerTests
    {
        private static (string, HealthIssue) Down(string key)
        {
            return ("Orbit", new HealthIssue(key, HealthLevel.Critical, "Uptime", $"{key} is down", null));
        }

        [Test]
        public void Update_FirstRun_DoesNotAnnounceExistingProblems()
        {
            AlertTracker tracker = new();

            Assert.That(tracker.Update([Down("api")]), Is.Empty);
        }

        [Test]
        public void Update_NewProblem_AnnouncedOnce()
        {
            AlertTracker tracker = new();
            tracker.Update([]);

            IReadOnlyList<AlertMessage> first = tracker.Update([Down("api")]);
            IReadOnlyList<AlertMessage> second = tracker.Update([Down("api")]);

            Assert.That(first.Single().Text, Is.EqualTo("🔴 Orbit: api is down"));
            Assert.That(second, Is.Empty);
        }

        [Test]
        public void Update_ProblemClears_AnnouncesResolution()
        {
            AlertTracker tracker = new();
            tracker.Update([]);
            tracker.Update([Down("api")]);

            IReadOnlyList<AlertMessage> resolved = tracker.Update([]);

            Assert.That(resolved.Single().Text, Is.EqualTo("✅ Orbit: resolved: api is down"));
        }
    }
}
