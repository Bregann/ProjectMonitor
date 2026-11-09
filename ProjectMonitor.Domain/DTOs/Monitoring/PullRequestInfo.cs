namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    public record PullRequestInfo(int Number, string Title, string Url, string Author, string Branch, bool IsDraft, DateTime CreatedUtc)
    {
        public bool IsRenovate => Branch.StartsWith("renovate/", StringComparison.Ordinal);
    }
}
