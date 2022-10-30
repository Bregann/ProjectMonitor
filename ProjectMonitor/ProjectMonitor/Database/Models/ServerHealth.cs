using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class SystemHealth
    {
        [Key]
        [Column("systemName")]
        public string SystemName { get; set; }

        [Column("systemUptime")]
        public TimeSpan SystemUptime { get; set; }

        [Column("lastUpdate")]
        public DateTime LastUpdate { get; set; }
    }
}
