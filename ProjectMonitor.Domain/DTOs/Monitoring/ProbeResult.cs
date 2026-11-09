namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    public record ProbeResult(bool IsUp, int? StatusCode, int? ResponseTimeMs, string? Error);
}
