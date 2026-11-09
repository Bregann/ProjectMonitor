using ProjectMonitor.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Domain.Database.Models
{
    /// <summary>
    /// Base class for every kind of check. Each type is a subclass stored in the same table (see AppDbContext),
    /// and has an ICheckEvaluator that turns it into a status. To add a new type: add a subclass, an evaluator,
    /// a CheckTypeEnum value and its fields in the check dialog.
    /// </summary>
    public abstract class ProjectCheck
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; } = null!;

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        /// <summary>How bad it is when this check fails: Warning or Critical.</summary>
        [Required]
        public HealthLevel FailureLevel { get; set; } = HealthLevel.Warning;

        [Required]
        public bool IsEnabled { get; set; } = true;

        [NotMapped]
        public abstract CheckTypeEnum CheckType { get; }
    }

    /// <summary>The monitor requests a URL every minute and records whether it answered.</summary>
    public class HttpCheck : ProjectCheck
    {
        [Required, MaxLength(500)]
        public required string Url { get; set; }

        /// <summary>When empty, any response below 500 counts as up.</summary>
        public List<int> ExpectedStatusCodes { get; set; } = [];

        public override CheckTypeEnum CheckType => CheckTypeEnum.Http;
    }

    /// <summary>The project must have reported something within the last N minutes.</summary>
    public class HeartbeatCheck : ProjectCheck
    {
        [Required]
        public int MaxAgeMinutes { get; set; } = 5;

        public override CheckTypeEnum CheckType => CheckTypeEnum.Heartbeat;
    }

    /// <summary>A rule on one value the project reports, e.g. "discordConnected is true".</summary>
    public class ValueCheck : ProjectCheck
    {
        [Required, MaxLength(100)]
        public required string ValueKey { get; set; }

        [Required]
        public ValueConditionEnum Condition { get; set; }

        /// <summary>Compared against for Equals / NotEquals / GreaterThan / LessThan; minutes for WithinMinutes.</summary>
        [MaxLength(200)]
        public string? ExpectedValue { get; set; }

        /// <summary>Optional: the value itself must have been reported within this many minutes.</summary>
        public int? MaxAgeMinutes { get; set; }

        public override CheckTypeEnum CheckType => CheckTypeEnum.Value;
    }
}
