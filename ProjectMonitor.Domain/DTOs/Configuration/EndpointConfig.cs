using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.DTOs.Configuration
{
    /// <summary>One website/API check (an HttpCheck row).</summary>
    public class EndpointConfig
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Url { get; set; } = "";

        /// <summary>When empty, any response below 500 counts as up: an API answering 401/404 on its root is still running.</summary>
        public List<int> ExpectedStatusCodes { get; set; } = [];

        public HealthLevel FailureLevel { get; set; } = HealthLevel.Critical;
    }
}
