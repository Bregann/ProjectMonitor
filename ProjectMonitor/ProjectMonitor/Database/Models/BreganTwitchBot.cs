using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class BreganTwitchBot
    {
        [Key]
        [Column("mode")]
        public string Mode { get; set; }

        [Column("usersInStream")]
        public long UsersInStream { get; set; }

        [Column("twitchIRCConnectionStatus")]
        public bool TwitchIRCConnectionStatus { get; set; }

        [Column("twitchPubSubConnectionStatus")]
        public bool TwitchPubSubConnectionStatus { get; set; }

        [Column("twitchApiKeyLastRefreshTime")]
        public DateTime TwitchApiKeyLastRefreshTime { get; set; }

        [Column("discordConnectionStatus")]
        public bool DiscordConnectionStatus { get; set; }

        [Column("streamAnnounced")]
        public bool StreamAnnounced { get; set; }

        [Column("streamLiveTime")]
        public DateTime StreamLiveTime { get; set; }

        [Column("streamStatus")]
        public bool StreamStatus { get; set; }

        [Column("streamUptime")]
        public TimeSpan StreamUptime { get; set; }

        [Column("dailyPointsEnabled")]
        public bool DailyPointsEnabled { get; set; }

        [Column("lastDiscordLeaderboardsUpdate")]
        public DateTime LastDiscordLeaderboardsUpdate { get; set; }

        [Column("lastHoursUpdate")]
        public DateTime LastHoursUpdate { get; set; }
    }
}
