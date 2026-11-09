namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    public record IssueInfo(int Number, string Title, string Url, DateTime UpdatedUtc);
}
