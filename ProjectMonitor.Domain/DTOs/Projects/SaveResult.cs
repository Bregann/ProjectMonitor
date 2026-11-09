namespace ProjectMonitor.Domain.DTOs.Projects
{
    /// <summary>Outcome of a save: Error is set (and nothing was saved) when the input wasn't valid.</summary>
    public record SaveResult(bool Success, string? Error = null, int? Id = null)
    {
        public static SaveResult Ok(int? id = null) => new(true, null, id);

        public static SaveResult Fail(string error) => new(false, error);
    }
}
