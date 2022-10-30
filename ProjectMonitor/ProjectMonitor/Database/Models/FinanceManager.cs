using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class FinanceManager
    {
        [Key]
        [Column("mode")]
        public string Mode { get; set; }

        [Column("lastTransactionUpdate")]
        public DateTime LastTransactionUpdate { get; set; }

        [Column("lastAPIRefresh")]
        public DateTime LastAPIRefresh { get; set; }

        [Column("lastAPIRefreshStatusCode")]
        public string LastAPIRefreshStatusCode { get; set; }
    }
}
