using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class Errors
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ErrorId { get; set; }

        [Required]
        public DateTime DateStarted { get; set; }

        public DateTime? DateEnded { get; set; }

        [Required]
        public TimeSpan DowntimeDuration { get; set; }

        [Required]
        public string ProjectName { get; set; }

        [Required]
        public string ErrorType { get; set; }

        [Required]
        public string ErrorDescription { get; set; }

        [Required]
        public bool EmailAlertSent { get; set; }

        [Required]
        public bool EmailResolvedAlertSent { get; set; }

        [Required]
        public bool TextAlertSent { get; set; }

        [Required]
        public bool TextResolvedAlertSent { get; set; }
    }
}