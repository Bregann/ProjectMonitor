namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    /// <summary>What a check evaluator gets to work with for one project.</summary>
    public record CheckEvaluationContext(
        IReadOnlyDictionary<string, ReportedValueInfo> LatestValues,
        DateTime? LastReportUtc,
        DateTime NowUtc);
}
