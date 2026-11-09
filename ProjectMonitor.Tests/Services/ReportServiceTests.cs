using Microsoft.EntityFrameworkCore;
using ProjectMonitor.Domain.Database.Context;
using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.DTOs.Reports;
using ProjectMonitor.Domain.Interfaces.Jobs;
using ProjectMonitor.Domain.Services.Reports;
using System.Text.Json;

namespace ProjectMonitor.Tests.Services
{
    public class ReportServiceTests
    {
        private sealed class FakeChecksService : IChecksService
        {
            public List<int> Evaluated { get; } = [];
            public Task EvaluateAll() => Task.CompletedTask;
            public Task EvaluateProject(int projectId)
            {
                Evaluated.Add(projectId);
                return Task.CompletedTask;
            }

            public Task PruneReportedValues() => Task.CompletedTask;
        }

        private AppDbContext _context = null!;
        private FakeChecksService _checks = null!;
        private ReportService _service = null!;
        private int _projectId;

        [SetUp]
        public async Task SetUp()
        {
            _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            Project project = new() { Slug = "bot", Name = "Bot", ApiKey = "secret-key", CreatedAtUtc = TestData.Now };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            _projectId = project.Id;
            _checks = new FakeChecksService();
            _service = new ReportService(_context, _checks);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private static ReportRequest Request(string json)
        {
            return new ReportRequest { Values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)! };
        }

        [Test]
        public async Task RecordReport_StoresEachValueAsTextAndEvaluatesChecks()
        {
            ReportResultEnum result = await _service.RecordReport("secret-key", Request("""{ "discordConnected": true, "queueLength": 3, "status": "running" }"""));

            Assert.That(result, Is.EqualTo(ReportResultEnum.Recorded));

            Dictionary<string, string> stored = await _context.ReportedValues.ToDictionaryAsync(x => x.Key, x => x.Value);
            Assert.That(stored, Is.EquivalentTo(new Dictionary<string, string> { ["discordConnected"] = "true", ["queueLength"] = "3", ["status"] = "running" }));
            Assert.That(_checks.Evaluated, Is.EqualTo(new[] { _projectId }));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("wrong-key")]
        public async Task RecordReport_BadKey_IsRejectedAndNothingStored(string? key)
        {
            ReportResultEnum result = await _service.RecordReport(key, Request("""{ "a": 1 }"""));

            Assert.That(result, Is.EqualTo(ReportResultEnum.InvalidApiKey));
            Assert.That(await _context.ReportedValues.CountAsync(), Is.Zero);
        }

        [Test]
        public async Task RecordReport_ObjectsAndArraysAreRejected()
        {
            ReportResultEnum result = await _service.RecordReport("secret-key", Request("""{ "a": { "nested": 1 } }"""));

            Assert.That(result, Is.EqualTo(ReportResultEnum.InvalidValues));
            Assert.That(await _context.ReportedValues.CountAsync(), Is.Zero);
        }

        [Test]
        public async Task RecordReport_EmptyReportCountsAsHeartbeat()
        {
            ReportResultEnum result = await _service.RecordReport("secret-key", new ReportRequest());

            Assert.That(result, Is.EqualTo(ReportResultEnum.Recorded));
            Assert.That(await _context.ReportedValues.Select(x => x.Key).SingleAsync(), Is.EqualTo("heartbeat"));
        }
    }
}
