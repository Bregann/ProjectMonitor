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
        IReadOnlyList<UptimeBucket> History)
    {
        /// <summary>How serious it is when this endpoint is down (set per check on the site).</summary>
        public Enums.HealthLevel FailureLevel { get; init; } = Enums.HealthLevel.Critical;
    }
}
