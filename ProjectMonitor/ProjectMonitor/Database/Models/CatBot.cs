using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class CatBot
    {
        [Key]
        [Column("mode")]
        public string Mode { get; set; }

        [Column("discordConnectionStatus")]
        public bool DiscordConnectionStatus { get; set; }

        [Column("lastTweet")]
        public DateTime LastTweet { get; set; }

        [Column("lastDiscordPost")]
        public DateTime LastDiscordPost { get; set; }
    }
}
