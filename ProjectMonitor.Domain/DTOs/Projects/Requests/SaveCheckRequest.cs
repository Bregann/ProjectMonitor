using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.DTOs.Projects.Requests
{
    /// <summary>Add or edit a check. Fill in the fields for <see cref="Type"/>; the rest are ignored.</summary>
    public class SaveCheckRequest
    {
        public CheckTypeEnum Type { get; set; } = CheckTypeEnum.Http;
        public string Name { get; set; } = "";
        public HealthLevel FailureLevel { get; set; } = HealthLevel.Critical;
        public bool IsEnabled { get; set; } = true;

        // Http
        public string? Url { get; set; }
        public List<int> ExpectedStatusCodes { get; set; } = [];

        // Heartbeat, and optionally Value
        public int? MaxAgeMinutes { get; set; }

        // Value
        public string? ValueKey { get; set; }
        public ValueConditionEnum Condition { get; set; } = ValueConditionEnum.IsTrue;
        public string? ExpectedValue { get; set; }
    }
}
