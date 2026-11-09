using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Domain.Database.Models
{
    public class Project
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>URL-safe identifier used in /projects/{slug}. Generated from the name and never changed.</summary>
        [Required, MaxLength(100)]
        public required string Slug { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        /// <summary>GitHub repository name under the configured owner, if the project has one.</summary>
        [MaxLength(100)]
        public string? Repository { get; set; }

        /// <summary>Sent by the project in the X-ProjectMonitor-ApiKey header when it reports values.</summary>
        [Required, MaxLength(100)]
        public required string ApiKey { get; set; }

        [Required]
        public DateTime CreatedAtUtc { get; set; }

        public virtual List<ProjectCheck> Checks { get; set; } = [];
    }
}
