namespace ProjectMonitor.Domain.DTOs.Projects
{
    public class ManageProjectDto
    {
        public int Id { get; set; }
        public required string Slug { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Repository { get; set; }
        public required string ApiKey { get; set; }
        public List<CheckDto> Checks { get; set; } = [];
    }
}
