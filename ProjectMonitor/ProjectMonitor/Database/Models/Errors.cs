using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class Errors
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("errorId")]
        public int ErrorId { get; set; }

        [Column("dateStarted")]
        public DateTime DateStarted { get; set; }

        [Column("dateEnded")]
        public DateTime? DateEnded { get; set; }

        [Column("downtimeDuration")]
        public TimeSpan DowntimeDuration { get; set; }

        [Column("projectName")]
        public string ProjectName { get; set; }

        [Column("errorType")]
        public string ErrorType { get; set; }

        [Column("errorDescription")]
        public string ErrorDescription { get; set; }

        [Column("alertSent")]
        public bool AlertSent { get; set; }

        [Column("resolvedAlertSent")]
        public bool ResolvedAlertSent { get; set; }
    }
}
