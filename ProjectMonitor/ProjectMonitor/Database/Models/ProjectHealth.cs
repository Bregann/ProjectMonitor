using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class ProjectHealth
    {
        [Key]
        [Column("projectName")]
        public string ProjectName { get; set; }

        [Column("cpuUsage")]
        public double CPUUsage { get; set; }

        [Column("ramUsage")]
        public long RAMUsage { get; set; }

        [Column("projectUptime")]
        public TimeSpan ProjectUptime { get; set; }

        [Column("lastUpdate")]
        public DateTime LastUpdate { get; set; }

        [Column("projectRunning")]
        public bool ProjectRunning { get; set; }
    }
}
