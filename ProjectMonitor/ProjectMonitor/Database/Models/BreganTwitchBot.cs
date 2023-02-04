using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class BreganTwitchBot
    {
        [Key]
        [Required]
        public string Mode { get; set; }

        [Required]
        public long UsersInStream { get; set; }

        [Required]
        public bool TwitchIRCConnectionStatus { get; set; }

        [Required]
        public bool TwitchPubSubConnectionStatus { get; set; }

        [Required]
        public DateTime TwitchApiKeyLastRefreshTime { get; set; }

        [Required]
        public bool DiscordConnectionStatus { get; set; }

        [Required]
        public bool StreamAnnounced { get; set; }

        [Required]
        public DateTime StreamLiveTime { get; set; }

        [Required]
        public bool StreamStatus { get; set; }

        [Required]
        public TimeSpan StreamUptime { get; set; }

        [Required]
        public bool DailyPointsEnabled { get; set; }

        [Required]
        public DateTime LastDiscordLeaderboardsUpdate { get; set; }

        [Required]
        public DateTime LastHoursUpdate { get; set; }
    }
}