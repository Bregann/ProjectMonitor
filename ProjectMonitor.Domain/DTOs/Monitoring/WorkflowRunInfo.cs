namespace ProjectMonitor.Domain.DTOs.Monitoring
{
    public record WorkflowRunInfo(
        long Id,
        string WorkflowName,
        string Title,
        string Branch,
        string Event,
        string Status,
        string? Conclusion,
        string Url,
        DateTime CreatedUtc,
        DateTime UpdatedUtc,
        string? Actor)
    {
        public bool IsActive => Status is "queued" or "in_progress" or "waiting" or "requested" or "pending";

        public bool IsFailure => Status == "completed" && Conclusion is "failure" or "timed_out" or "startup_failure";

        public bool IsSuccess => Status == "completed" && Conclusion == "success";
    }
}
