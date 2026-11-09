using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    /// <param name="Key">Stable identifier, so alerts can tell "still the same problem" from "a new one".</param>
    public record HealthIssue(string Key, HealthLevel Level, string Category, string Message, string? Url);
}
