using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Interfaces.Helpers;

namespace ProjectMonitor.Domain.Helpers.Checks
{
    public class HeartbeatCheckEvaluator : ICheckEvaluator
    {
        public CheckTypeEnum CheckType => CheckTypeEnum.Heartbeat;

        public CheckStatus Evaluate(ProjectCheck check, CheckEvaluationContext context)
        {
            HeartbeatCheck heartbeat = (HeartbeatCheck)check;
            string rule = CheckDescriptionHelper.Describe(check);

            if (context.LastReportUtc is not DateTime lastReport)
            {
                return new CheckStatus(check.Id, check.Name, CheckType, check.FailureLevel, rule, "Has never reported", null, null);
            }

            TimeSpan age = context.NowUtc - lastReport;

            return age > TimeSpan.FromMinutes(heartbeat.MaxAgeMinutes)
                ? new CheckStatus(check.Id, check.Name, CheckType, check.FailureLevel, rule, $"No report for {(int)age.TotalMinutes} minutes", null, lastReport)
                : new CheckStatus(check.Id, check.Name, CheckType, HealthLevel.Healthy, rule, "Reporting", null, lastReport);
        }
    }
}
