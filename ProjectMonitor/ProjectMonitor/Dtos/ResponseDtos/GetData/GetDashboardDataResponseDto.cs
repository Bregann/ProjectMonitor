namespace ProjectMonitor.Api.Models.GetData
{
    public class GetDashboardDataResponseDto
    {
        public List<ProjectStatus> ProjectStatus { get; set; }
        public TwitchBot TwitchBot { get; set; }
        public RetroAchievementsTracker RetroAchievementsTracker { get; set; }
        public FinanceManager FinanceManager { get; set; }
        public CatBot CatBot { get; set; }
        public string LastUpdate { get; set; }
    }

    public class ProjectStatus
    {
        public string ProjectName { get; set; }
        public double CPUUsage { get; set; }
        public long RAMUsage { get; set; }
        public string ProjectUptime { get; set; }
        public bool ProjectRunning { get; set; }
    }

    public class TwitchBot
    {
        public long UsersInStream { get; set; }
        public bool TwitchIRCConnectionStatus { get; set; }
        public bool TwitchPubSubConnectionStatus { get; set; }
        public string TwitchApiKeyLastRefreshTime { get; set; }
        public bool DiscordConnectionStatus { get; set; }
        public bool StreamAnnounced { get; set; }
        public bool StreamStatus { get; set; }
        public string StreamUptime { get; set; }
        public bool DailyPointsEnabled { get; set; }
        public string LastDiscordLeaderboardsUpdate { get; set; }
        public string LastHoursUpdate { get; set; }
    }

    public class RetroAchievementsTracker
    {
        public long TotalGames { get; set; }
        public long TotalUsers { get; set; }
        public string LastGameUpdate { get; set; }
        public long GamesUpdated { get; set; }
    }

    public class FinanceManager
    {
        public string LastTransactionUpdate { get; set; }
        public string LastAPIRefresh { get; set; }
        public string LastAPIRefreshStatusCode { get; set; }
    }

    public class CatBot
    {
        public bool DiscordConnectionStatus { get; set; }
        public string LastTweet { get; set; }
        public string LastDiscordPost { get; set; }
    }
}