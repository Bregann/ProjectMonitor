using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class CatBot
    {
        [Key]
        [Required]
        public string Mode { get; set; }

        [Required]
        public bool DiscordConnectionStatus { get; set; }

        [Required]
        public DateTime LastTweet { get; set; }

        [Required]
        public DateTime LastDiscordPost { get; set; }
    }
}