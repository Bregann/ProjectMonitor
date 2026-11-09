using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    public record ProjectHealth(HealthLevel Level, IReadOnlyList<HealthIssue> Issues, bool HasData);
}
