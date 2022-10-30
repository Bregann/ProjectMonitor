using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMonitor.Api.Database.Models
{
    public class RetroAchievementsTracker
    {
        [Key]
        [Column("mode")]
        public string Mode { get; set; }

        [Column("totalGames")]
        public long TotalGames { get; set; }

        [Column("totalUsers")]
        public long TotalUsers { get; set; }

        [Column("lastGameUpdate")]
        public DateTime LastGameUpdate { get; set; }

        [Column("gamesUpdated")]
        public long GamesUpdated { get; set; }
    }
}
