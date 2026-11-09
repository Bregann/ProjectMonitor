using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Helpers.Checks;

namespace ProjectMonitor.Tests.Helpers
{
    public class CheckEvaluatorTests
    {
        private static CheckEvaluationContext Context(params (string Key, string Value, int MinutesAgo)[] values)
        {
            Dictionary<string, ReportedValueInfo> latest = values.ToDictionary(
                x => x.Key,
                x => new ReportedValueInfo(x.Key, x.Value, TestData.Now.AddMinutes(-x.MinutesAgo)));

            return new CheckEvaluationContext(latest, latest.Count == 0 ? null : latest.Values.Max(x => x.ReportedAtUtc), TestData.Now);
        }

        private static ValueCheck Value(ValueConditionEnum condition, string? expected = null, int? maxAge = null)
        {
            return new ValueCheck { Id = 1, Name = "Check", ValueKey = "v", Condition = condition, ExpectedValue = expected, MaxAgeMinutes = maxAge, FailureLevel = HealthLevel.Critical };
        }

        [TestCase(ValueConditionEnum.IsTrue, null, "true", true)]
        [TestCase(ValueConditionEnum.IsTrue, null, "false", false)]
        [TestCase(ValueConditionEnum.IsTrue, null, "1", true)]
        [TestCase(ValueConditionEnum.IsFalse, null, "false", true)]
        [TestCase(ValueConditionEnum.EqualTo, "running", "Running", true)]
        [TestCase(ValueConditionEnum.EqualTo, "3", "3.0", true)]
        [TestCase(ValueConditionEnum.NotEqualTo, "error", "ok", true)]
        [TestCase(ValueConditionEnum.NotEqualTo, "error", "error", false)]
        [TestCase(ValueConditionEnum.GreaterThan, "10", "11", true)]
        [TestCase(ValueConditionEnum.GreaterThan, "10", "10", false)]
        [TestCase(ValueConditionEnum.LessThan, "50", "3", true)]
        [TestCase(ValueConditionEnum.LessThan, "50", "abc", false)]
        [TestCase(ValueConditionEnum.WithinMinutes, "60", "2026-11-09T11:30:00Z", true)]
        [TestCase(ValueConditionEnum.WithinMinutes, "60", "2026-11-09T10:30:00Z", false)]
        [TestCase(ValueConditionEnum.WithinMinutes, "60", "not a date", false)]
        public void ValueCheck_Conditions(ValueConditionEnum condition, string? expected, string reported, bool passes)
        {
            CheckStatus status = new ValueCheckEvaluator().Evaluate(Value(condition, expected), Context(("v", reported, 1)));

            Assert.That(status.Level, Is.EqualTo(passes ? HealthLevel.Healthy : HealthLevel.Critical), status.Message);
            Assert.That(status.CurrentValue, Is.EqualTo(reported));
        }

        [Test]
        public void ValueCheck_NeverReported_Fails()
        {
            CheckStatus status = new ValueCheckEvaluator().Evaluate(Value(ValueConditionEnum.IsTrue), Context());

            Assert.That(status.Level, Is.EqualTo(HealthLevel.Critical));
            Assert.That(status.Message, Is.EqualTo("v has never been reported"));
        }

        [Test]
        public void ValueCheck_StaleValue_FailsEvenIfItWouldPass()
        {
            CheckStatus status = new ValueCheckEvaluator().Evaluate(Value(ValueConditionEnum.IsTrue, maxAge: 5), Context(("v", "true", 30)));

            Assert.That(status.Level, Is.EqualTo(HealthLevel.Critical));
            Assert.That(status.Message, Does.Contain("hasn't been reported for 30 minutes"));
        }

        [Test]
        public void ValueCheck_UsesConfiguredFailureLevel()
        {
            ValueCheck check = Value(ValueConditionEnum.IsTrue);
            check.FailureLevel = HealthLevel.Warning;

            CheckStatus status = new ValueCheckEvaluator().Evaluate(check, Context(("v", "false", 1)));

            Assert.That(status.Level, Is.EqualTo(HealthLevel.Warning));
        }

        [TestCase(2, true)]
        [TestCase(10, false)]
        public void Heartbeat_PassesOnlyWhenReportedRecently(int minutesAgo, bool passes)
        {
            HeartbeatCheck check = new() { Id = 2, Name = "Bot running", MaxAgeMinutes = 5, FailureLevel = HealthLevel.Critical };

            CheckStatus status = new HeartbeatCheckEvaluator().Evaluate(check, Context(("anything", "1", minutesAgo)));

            Assert.That(status.Level, Is.EqualTo(passes ? HealthLevel.Healthy : HealthLevel.Critical));
        }

        [Test]
        public void Heartbeat_NeverReported_Fails()
        {
            HeartbeatCheck check = new() { Id = 2, Name = "Bot running", MaxAgeMinutes = 5, FailureLevel = HealthLevel.Warning };

            CheckStatus status = new HeartbeatCheckEvaluator().Evaluate(check, Context());

            Assert.That(status.Level, Is.EqualTo(HealthLevel.Warning));
            Assert.That(status.Message, Is.EqualTo("Has never reported"));
        }
    }
}
