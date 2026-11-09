using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.DTOs.Projects
{
    /// <summary>A check as shown and edited on the Manage projects page. Only the fields for its type are set.</summary>
    public class CheckDto
    {
        public int Id { get; set; }
        public CheckTypeEnum Type { get; set; }
        public required string Name { get; set; }
        public HealthLevel FailureLevel { get; set; }
        public bool IsEnabled { get; set; }

        /// <summary>The rule in words, e.g. "GET https://… answers" or "discordConnected is true".</summary>
        public required string Summary { get; set; }

        public string? Url { get; set; }
        public List<int> ExpectedStatusCodes { get; set; } = [];
        public int? MaxAgeMinutes { get; set; }
        public string? ValueKey { get; set; }
        public ValueConditionEnum? Condition { get; set; }
        public string? ExpectedValue { get; set; }
    }
}
