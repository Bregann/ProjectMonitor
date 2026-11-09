using ProjectMonitor.Domain.DTOs.Monitoring;

namespace ProjectMonitor.Domain.DTOs.Dashboard
{
    public class AtAGlanceDto
    {
        public required string SummaryLine { get; set; }
        public required SummaryTilesDto Tiles { get; set; }
        public required List<ProjectRowDto> Projects { get; set; }
        public required List<AttentionItemDto> NeedsAttention { get; set; }
        public required List<ActiveRunDto> RunningNow { get; set; }
    }

    public class AttentionItemDto
    {
        public required string ProjectName { get; set; }
        public required string ProjectSlug { get; set; }
        public required HealthIssue Issue { get; set; }
    }

    public class ActiveRunDto
    {
        public required string ProjectName { get; set; }
        public required WorkflowRunInfo Run { get; set; }
    }
}
