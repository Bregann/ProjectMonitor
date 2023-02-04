using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class SystemHealth
    {
        [Key]
        [Required]
        public string SystemName { get; set; }

        [Required]
        public TimeSpan SystemUptime { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }

        [Required]
        public bool SystemRunning { get; set; }
    }
}