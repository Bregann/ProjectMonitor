using Microsoft.EntityFrameworkCore;
using ProjectMonitor.Domain.Database.Context;
using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.DTOs.Reports;
using ProjectMonitor.Domain.Interfaces.Api.Reports;
using ProjectMonitor.Domain.Interfaces.Jobs;
using System.Text.Json;

namespace ProjectMonitor.Domain.Services.Reports
{
    public class ReportService(AppDbContext context, IChecksService checksService) : IReportService
    {
        public const int MaxValuesPerReport = 100;
        public const int MaxKeyLength = 100;
        public const int MaxValueLength = 1000;

        public async Task<ReportResultEnum> RecordReport(string? apiKey, ReportRequest request)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return ReportResultEnum.InvalidApiKey;
            }

            int? projectId = await context.Projects
                .Where(x => x.ApiKey == apiKey)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync();

            if (projectId is null)
            {
                return ReportResultEnum.InvalidApiKey;
            }

            if (request.Values.Count > MaxValuesPerReport || request.Values.Keys.Any(k => string.IsNullOrWhiteSpace(k) || k.Length > MaxKeyLength))
            {
                return ReportResultEnum.InvalidValues;
            }

            DateTime nowUtc = DateTime.UtcNow;
            List<ReportedValue> values = [];

            foreach ((string key, JsonElement element) in request.Values)
            {
                string? value = ToText(element);

                if (value is null || value.Length > MaxValueLength)
                {
                    return ReportResultEnum.InvalidValues;
                }

                values.Add(new ReportedValue { ProjectId = projectId.Value, Key = key.Trim(), Value = value, ReportedAtUtc = nowUtc });
            }

            // An empty report is still a heartbeat, so record that the project checked in
            if (values.Count == 0)
            {
                values.Add(new ReportedValue { ProjectId = projectId.Value, Key = "heartbeat", Value = "true", ReportedAtUtc = nowUtc });
            }

            context.ReportedValues.AddRange(values);
            await context.SaveChangesAsync();

            await checksService.EvaluateProject(projectId.Value);
            return ReportResultEnum.Recorded;
        }

        /// <summary>Strings as-is, everything else (numbers, true/false) as its JSON text. Objects and arrays aren't allowed.</summary>
        private static string? ToText(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False => element.GetRawText(),
                _ => null,
            };
        }
    }
}
