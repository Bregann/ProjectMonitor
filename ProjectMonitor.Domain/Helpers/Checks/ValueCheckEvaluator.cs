using ProjectMonitor.Domain.Database.Models;
using ProjectMonitor.Domain.DTOs.Monitoring;
using ProjectMonitor.Domain.Enums;
using ProjectMonitor.Domain.Interfaces.Helpers;
using System.Globalization;

namespace ProjectMonitor.Domain.Helpers.Checks
{
    public class ValueCheckEvaluator : ICheckEvaluator
    {
        public CheckTypeEnum CheckType => CheckTypeEnum.Value;

        public CheckStatus Evaluate(ProjectCheck check, CheckEvaluationContext context)
        {
            ValueCheck valueCheck = (ValueCheck)check;
            string rule = CheckDescriptionHelper.Describe(check);

            CheckStatus Fail(string message, ReportedValueInfo? value) =>
                new(check.Id, check.Name, CheckType, check.FailureLevel, rule, message, value?.Value, value?.ReportedAtUtc);

            if (!context.LatestValues.TryGetValue(valueCheck.ValueKey, out ReportedValueInfo? value))
            {
                return Fail($"{valueCheck.ValueKey} has never been reported", null);
            }

            if (valueCheck.MaxAgeMinutes is int maxAge && context.NowUtc - value.ReportedAtUtc > TimeSpan.FromMinutes(maxAge))
            {
                return Fail($"{valueCheck.ValueKey} hasn't been reported for {(int)(context.NowUtc - value.ReportedAtUtc).TotalMinutes} minutes", value);
            }

            string? problem = FindProblem(valueCheck, value.Value, context.NowUtc);

            return problem is null
                ? new CheckStatus(check.Id, check.Name, CheckType, HealthLevel.Healthy, rule, "OK", value.Value, value.ReportedAtUtc)
                : Fail(problem, value);
        }

        /// <summary>Returns why the value fails the rule, or null when it passes.</summary>
        public static string? FindProblem(ValueCheck check, string value, DateTime nowUtc)
        {
            string key = check.ValueKey;
            string expected = check.ExpectedValue ?? "";

            switch (check.Condition)
            {
                case ValueConditionEnum.IsTrue:
                case ValueConditionEnum.IsFalse:
                    bool wanted = check.Condition == ValueConditionEnum.IsTrue;
                    return TryParseBool(value) is bool actual
                        ? actual == wanted ? null : $"{key} is {(actual ? "true" : "false")}"
                        : $"{key} is \"{value}\", not true/false";

                case ValueConditionEnum.EqualTo:
                    return ValuesEqual(value, expected) ? null : $"{key} is {value}, expected {expected}";

                case ValueConditionEnum.NotEqualTo:
                    return ValuesEqual(value, expected) ? $"{key} is {value}" : null;

                case ValueConditionEnum.GreaterThan:
                case ValueConditionEnum.LessThan:
                    if (!TryParseNumber(value, out double number) || !TryParseNumber(expected, out double limit))
                    {
                        return $"{key} is \"{value}\", not a number";
                    }

                    bool passes = check.Condition == ValueConditionEnum.GreaterThan ? number > limit : number < limit;
                    return passes ? null : $"{key} is {value}, expected {(check.Condition == ValueConditionEnum.GreaterThan ? "more" : "less")} than {expected}";

                case ValueConditionEnum.WithinMinutes:
                    if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out DateTime timestamp))
                    {
                        return $"{key} is \"{value}\", not a date/time";
                    }

                    if (!TryParseNumber(expected, out double minutes))
                    {
                        return "The check's minutes setting isn't a number";
                    }

                    TimeSpan age = nowUtc - timestamp;
                    return age <= TimeSpan.FromMinutes(minutes) ? null : $"{key} was {(int)age.TotalMinutes} minutes ago (limit {minutes:0})";

                default:
                    return $"Unknown condition {check.Condition}";
            }
        }

        private static bool? TryParseBool(string value)
        {
            return value.Trim().ToLowerInvariant() switch
            {
                "true" or "1" or "yes" => true,
                "false" or "0" or "no" => false,
                _ => null,
            };
        }

        private static bool TryParseNumber(string value, out double number)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
        }

        private static bool ValuesEqual(string a, string b)
        {
            return TryParseNumber(a, out double x) && TryParseNumber(b, out double y)
                ? x == y
                : string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
