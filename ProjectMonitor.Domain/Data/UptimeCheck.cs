namespace ProjectMonitor.Domain.Data
{
    public class UptimeCheck
    {
        public long Id { get; set; }

        /// <summary>"{projectSlug}/{endpointName}", so history survives URL changes.</summary>
        public required string EndpointKey { get; set; }

        public DateTime CheckedAtUtc { get; set; }

        public bool IsUp { get; set; }

        public int? StatusCode { get; set; }

        public int? ResponseTimeMs { get; set; }

        public string? Error { get; set; }
    }
}
