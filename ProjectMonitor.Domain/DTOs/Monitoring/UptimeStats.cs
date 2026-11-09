namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    public record UptimeStats(double? Uptime24h, double? Uptime7d, IReadOnlyList<UptimeBucket> Last24Hours);
}
