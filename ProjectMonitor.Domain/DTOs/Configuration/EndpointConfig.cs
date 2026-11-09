namespace ProjectMonitor.Domain.DTOs.Configuration
{
    public class EndpointConfig
    {
        public string Name { get; set; } = "";

        public string Url { get; set; } = "";

        /// <summary>
        /// Status codes that count as "up". When empty, any response below 500 counts as up:
        /// an API answering 401/404 on its root is still running.
        /// </summary>
        public List<int> ExpectedStatusCodes { get; set; } = [];
    }
}
