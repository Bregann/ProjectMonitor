using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.Helpers
{
    /// <summary>Describes a check's rule in words for the site, e.g. "discordConnected is true".</summary>
    public static class CheckDescriptionHelper
    {
        public static string Describe(ProjectCheck check)
        {
            return check switch
            {
                HttpCheck http => http.ExpectedStatusCodes.Count > 0
                    ? $"{http.Url} answers with {string.Join(" or ", http.ExpectedStatusCodes)}"
                    : $"{http.Url} answers",
                HeartbeatCheck heartbeat => $"Reports at least every {heartbeat.MaxAgeMinutes} min",
                ValueCheck value => DescribeValue(value),
                _ => check.Name,
            };
        }

        private static string DescribeValue(ValueCheck check)
        {
            string rule = check.Condition switch
            {
                ValueConditionEnum.IsTrue => $"{check.ValueKey} is true",
                ValueConditionEnum.IsFalse => $"{check.ValueKey} is false",
                ValueConditionEnum.EqualTo => $"{check.ValueKey} = {check.ExpectedValue}",
                ValueConditionEnum.NotEqualTo => $"{check.ValueKey} ≠ {check.ExpectedValue}",
                ValueConditionEnum.GreaterThan => $"{check.ValueKey} > {check.ExpectedValue}",
                ValueConditionEnum.LessThan => $"{check.ValueKey} < {check.ExpectedValue}",
                ValueConditionEnum.WithinMinutes => $"{check.ValueKey} within the last {check.ExpectedValue} min",
                _ => check.ValueKey,
            };

            return check.MaxAgeMinutes is int maxAge ? $"{rule} (reported in the last {maxAge} min)" : rule;
        }
    }
}
