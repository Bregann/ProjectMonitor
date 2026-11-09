namespace ProjectMonitor.Domain.DTOs.Dashboard
{
    public class DataFreshnessDto
    {
        public DateTime? LastGitHubPollUtc { get; set; }
        public DateTime? LastUptimeCheckUtc { get; set; }
        public DateTime? LatestUtc => new[] { LastGitHubPollUtc, LastUptimeCheckUtc }.Max();
    }
}
