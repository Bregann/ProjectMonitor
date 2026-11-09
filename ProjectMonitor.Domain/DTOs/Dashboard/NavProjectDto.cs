using ProjectMonitor.Domain.Enums;

namespace ProjectMonitor.Domain.DTOs.Dashboard
{
    public class NavProjectDto
    {
        public required string Slug { get; set; }
        public required string Name { get; set; }
        public HealthLevel Level { get; set; }
        public bool HasData { get; set; }
    }
}
