namespace ProjectMonitor.Domain.DTOs.Projects.Requests
{
    public class SaveProjectRequest
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? Repository { get; set; }
    }
}
