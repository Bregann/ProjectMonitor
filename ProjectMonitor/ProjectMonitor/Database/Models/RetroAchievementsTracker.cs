using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class RetroAchievementsTracker
    {
        [Key]
        [Required]
        public string Mode { get; set; }

        [Required]
        public long TotalGames { get; set; }

        [Required]
        public long TotalUsers { get; set; }

        [Required]
        public DateTime LastGameUpdate { get; set; }

        [Required]
        public long GamesUpdated { get; set; }
    }
}