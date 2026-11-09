namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    public record EndpointStatus(
        string Key,
        string Name,
        string Url,
        bool? IsUp,
        DateTime? LastCheckedUtc,
        int? StatusCode,
        int? ResponseTimeMs,
        string? Error,
        double? Uptime24h,
        double? Uptime7d,
        DateTime? DownSinceUtc,
        IReadOnlyList<UptimeBucket> History);
}
