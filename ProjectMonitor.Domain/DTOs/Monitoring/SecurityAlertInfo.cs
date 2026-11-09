namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    public record SecurityAlertInfo(
        int Number,
        string Severity,
        string Summary,
        string Package,
        string Ecosystem,
        string? ManifestPath,
        string? Relationship,
        string? FixedIn,
        string Url,
        DateTime CreatedUtc);
}
