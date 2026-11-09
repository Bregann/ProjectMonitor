using ProjectMonitor.Domain.DTOs.Projects.Requests;
using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Services.Projects;

namespace ProjectMonitor.Tests.Services
{
    public class ProjectsServiceValidationTests
    {
        [TestCase("", null, false)]
        [TestCase("Orbit", null, true)]
        [TestCase("Orbit", "Orbit", true)]
        [TestCase("Orbit", "Bregann/Orbit", false)]
        [TestCase("Orbit", "https://github.com/Bregann/Orbit", false)]
        public void ValidateProject(string name, string? repository, bool valid)
        {
            string? error = ProjectsService.ValidateProject(new SaveProjectRequest { Name = name, Repository = repository });

            Assert.That(error is null, Is.EqualTo(valid), error);
        }

        [TestCase("https://orbitapi.bregan.me", true)]
        [TestCase("http://localhost:8080/health", true)]
        [TestCase("orbitapi.bregan.me", false)]
        [TestCase("ftp://example.com", false)]
        public void ValidateCheck_HttpNeedsFullUrl(string url, bool valid)
        {
            string? error = ProjectsService.ValidateCheck(new SaveCheckRequest { Type = CheckTypeEnum.Http, Name = "API", Url = url });

            Assert.That(error is null, Is.EqualTo(valid), error);
        }

        [Test]
        public void ValidateCheck_HeartbeatNeedsMinutes()
        {
            Assert.That(ProjectsService.ValidateCheck(new SaveCheckRequest { Type = CheckTypeEnum.Heartbeat, Name = "Bot" }), Is.Not.Null);
            Assert.That(ProjectsService.ValidateCheck(new SaveCheckRequest { Type = CheckTypeEnum.Heartbeat, Name = "Bot", MaxAgeMinutes = 5 }), Is.Null);
        }

        [TestCase(ValueConditionEnum.IsTrue, null, true)]
        [TestCase(ValueConditionEnum.EqualTo, null, false)]
        [TestCase(ValueConditionEnum.EqualTo, "running", true)]
        [TestCase(ValueConditionEnum.GreaterThan, "lots", false)]
        [TestCase(ValueConditionEnum.LessThan, "50", true)]
        [TestCase(ValueConditionEnum.WithinMinutes, "1445", true)]
        public void ValidateCheck_ValueConditionsNeedTheRightKindOfValue(ValueConditionEnum condition, string? expected, bool valid)
        {
            SaveCheckRequest request = new() { Type = CheckTypeEnum.Value, Name = "Check", ValueKey = "v", Condition = condition, ExpectedValue = expected };

            Assert.That(ProjectsService.ValidateCheck(request) is null, Is.EqualTo(valid));
        }

        [Test]
        public void ValidateCheck_FailureLevelMustBeWarningOrCritical()
        {
            SaveCheckRequest request = new() { Type = CheckTypeEnum.Heartbeat, Name = "Bot", MaxAgeMinutes = 5, FailureLevel = HealthLevel.Info };

            Assert.That(ProjectsService.ValidateCheck(request), Is.Not.Null);
        }
    }
}
