using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class ProjectHealth
    {
        [Key]
        [Required]
        public string ProjectName { get; set; }

        [Required]
        public double CPUUsage { get; set; }

        [Required]
        public long RAMUsage { get; set; }

        [Required]
        public TimeSpan ProjectUptime { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }

        [Required]
        public bool ProjectRunning { get; set; }
    }
}