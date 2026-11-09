using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Domain.Database.Models
{
    /// <summary>One value a project sent to POST /api/report. Every report is kept (for history) until pruned.</summary>
    public class ReportedValue
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; } = null!;

        [Required, MaxLength(100)]
        public required string Key { get; set; }

        /// <summary>Stored as text: true/false, numbers and ISO timestamps are parsed when checks run.</summary>
        [Required, MaxLength(1000)]
        public required string Value { get; set; }

        [Required]
        public DateTime ReportedAtUtc { get; set; }
    }
}
