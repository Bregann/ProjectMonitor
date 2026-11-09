using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Domain.Database.Models
{
    public class UptimeCheck
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>"{projectSlug}/{endpointName}", so history survives URL changes.</summary>
        [Required, MaxLength(200)]
        public required string EndpointKey { get; set; }

        [Required]
        public DateTime CheckedAtUtc { get; set; }

        [Required]
        public bool IsUp { get; set; }

        public int? StatusCode { get; set; }

        public int? ResponseTimeMs { get; set; }

        [MaxLength(500)]
        public string? Error { get; set; }
    }
}
