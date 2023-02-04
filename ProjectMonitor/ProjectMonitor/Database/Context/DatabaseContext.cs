using Microsoft.EntityFrameworkCore;
using ProjectMonitor.Api.Database.Models;

namespace ProjectMonitor.Api.Database.Context
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ProjectHealth> ProjectHealth { get; set; }
        public DbSet<SystemHealth> SystemHealth { get; set; }
        public DbSet<BreganTwitchBot> BreganTwitchBot { get; set; }
        public DbSet<RetroAchievementsTracker> RetroAchievementsTracker { get; set; }
        public DbSet<FinanceManager> FinanceManager { get; set; }
        public DbSet<CatBot> CatBot { get; set; }
        public DbSet<Errors> Errors { get; set; }
        public DbSet<Config> Config { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Config>().HasData(
                new Config
                {
                    RowId = 1,
                    ApiKey = "",
                    FromEmailAddress = "",
                    ToEmailAddress = "",
                    FromEmailAddressName = "",
                    ToEmailAddressName = "",
                    PMErrorsResolvedTemplateId = "",
                    PMErrorsTemplateId = "",
                    HFConnectionString = "",
                    ChatId = 0,
                    MMSApiKey = ""
                });

            modelBuilder.Entity<BreganTwitchBot>().HasData(
                new BreganTwitchBot
                {
                    Mode = "debug",
                    DailyPointsEnabled = false,
                    DiscordConnectionStatus = true,
                    LastDiscordLeaderboardsUpdate = new DateTime(0, DateTimeKind.Utc),
                    LastHoursUpdate = new DateTime(0, DateTimeKind.Utc),
                    StreamAnnounced = false,
                    StreamStatus = false,
                    StreamUptime = TimeSpan.FromSeconds(0),
                    TwitchApiKeyLastRefreshTime = new DateTime(0, DateTimeKind.Utc),
                    TwitchIRCConnectionStatus = true,
                    TwitchPubSubConnectionStatus = true,
                    UsersInStream = 0,
                    StreamLiveTime = new DateTime(0, DateTimeKind.Utc)
                },

                new BreganTwitchBot
                {
                    Mode = "release",
                    DailyPointsEnabled = false,
                    DiscordConnectionStatus = true,
                    LastDiscordLeaderboardsUpdate = new DateTime(0, DateTimeKind.Utc),
                    LastHoursUpdate = new DateTime(0, DateTimeKind.Utc),
                    StreamAnnounced = false,
                    StreamStatus = false,
                    StreamUptime = TimeSpan.FromSeconds(0),
                    TwitchApiKeyLastRefreshTime = new DateTime(0, DateTimeKind.Utc),
                    TwitchIRCConnectionStatus = true,
                    TwitchPubSubConnectionStatus = true,
                    UsersInStream = 0,
                    StreamLiveTime = new DateTime(0, DateTimeKind.Utc)
                });

            modelBuilder.Entity<RetroAchievementsTracker>().HasData(
                new RetroAchievementsTracker
                {
                    Mode = "debug",
                    GamesUpdated = 0,
                    LastGameUpdate = new DateTime(0, DateTimeKind.Utc),
                    TotalGames = 0,
                    TotalUsers = 0
                },
                new RetroAchievementsTracker
                {
                    Mode = "release",
                    GamesUpdated = 0,
                    LastGameUpdate = new DateTime(0, DateTimeKind.Utc),
                    TotalGames = 0,
                    TotalUsers = 0
                });

            modelBuilder.Entity<FinanceManager>().HasData(
                new FinanceManager
                {
                    Mode = "debug",
                    LastAPIRefresh = new DateTime(0, DateTimeKind.Utc),
                    LastTransactionUpdate = new DateTime(0, DateTimeKind.Utc),
                    LastAPIRefreshStatusCode = "Success"
                },
                new FinanceManager
                {
                    Mode = "release",
                    LastAPIRefresh = new DateTime(0, DateTimeKind.Utc),
                    LastTransactionUpdate = new DateTime(0, DateTimeKind.Utc),
                    LastAPIRefreshStatusCode = "Success"
                });

            modelBuilder.Entity<CatBot>().HasData(
                new CatBot
                {
                    Mode = "debug",
                    DiscordConnectionStatus = true,
                    LastDiscordPost = new DateTime(0, DateTimeKind.Utc),
                    LastTweet = new DateTime(0, DateTimeKind.Utc)
                },
                new CatBot
                {
                    Mode = "release",
                    DiscordConnectionStatus = true,
                    LastDiscordPost = new DateTime(0, DateTimeKind.Utc),
                    LastTweet = new DateTime(0, DateTimeKind.Utc)
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(AppConfig.ConnectionString);
    }
}