namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    /// <summary>One bar in the uptime history strip: share of successful checks in that hour, or null if there were none.</summary>
    public record UptimeBucket(DateTime StartUtc, double? UptimePercent);
}
