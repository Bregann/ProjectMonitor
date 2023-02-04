using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class FinanceManager
    {
        [Key]
        [Required]
        public string Mode { get; set; }

        [Required]
        public DateTime LastTransactionUpdate { get; set; }

        [Required]
        public DateTime LastAPIRefresh { get; set; }

        [Required]
        public string LastAPIRefreshStatusCode { get; set; }
    }
}