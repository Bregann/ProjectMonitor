using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    /// <summary>The current result of one check. Level is Healthy when it passes.</summary>
    /// <param name="Rule">What the check expects, in words, e.g. "discordConnected is true".</param>
    /// <param name="CurrentValue">What was last reported, if the check looks at a value.</param>
    public record CheckStatus(
        int CheckId,
        string Name,
        CheckTypeEnum Type,
        HealthLevel Level,
        string Rule,
        string Message,
        string? CurrentValue,
        DateTime? LastReportedUtc);
}
