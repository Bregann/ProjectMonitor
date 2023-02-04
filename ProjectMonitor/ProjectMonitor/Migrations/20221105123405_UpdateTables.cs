using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class UpdateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "systemUptime",
                table: "SystemHealth",
                newName: "SystemUptime");

            migrationBuilder.RenameColumn(
                name: "systemRunning",
                table: "SystemHealth",
                newName: "SystemRunning");

            migrationBuilder.RenameColumn(
                name: "lastUpdate",
                table: "SystemHealth",
                newName: "LastUpdate");

            migrationBuilder.RenameColumn(
                name: "systemName",
                table: "SystemHealth",
                newName: "SystemName");

            migrationBuilder.RenameColumn(
                name: "totalUsers",
                table: "RetroAchievementsTracker",
                newName: "TotalUsers");

            migrationBuilder.RenameColumn(
                name: "totalGames",
                table: "RetroAchievementsTracker",
                newName: "TotalGames");

            migrationBuilder.RenameColumn(
                name: "lastGameUpdate",
                table: "RetroAchievementsTracker",
                newName: "LastGameUpdate");

            migrationBuilder.RenameColumn(
                name: "gamesUpdated",
                table: "RetroAchievementsTracker",
                newName: "GamesUpdated");

            migrationBuilder.RenameColumn(
                name: "mode",
                table: "RetroAchievementsTracker",
                newName: "Mode");

            migrationBuilder.RenameColumn(
                name: "ramUsage",
                table: "ProjectHealth",
                newName: "RAMUsage");

            migrationBuilder.RenameColumn(
                name: "projectUptime",
                table: "ProjectHealth",
                newName: "ProjectUptime");

            migrationBuilder.RenameColumn(
                name: "projectRunning",
                table: "ProjectHealth",
                newName: "ProjectRunning");

            migrationBuilder.RenameColumn(
                name: "lastUpdate",
                table: "ProjectHealth",
                newName: "LastUpdate");

            migrationBuilder.RenameColumn(
                name: "cpuUsage",
                table: "ProjectHealth",
                newName: "CPUUsage");

            migrationBuilder.RenameColumn(
                name: "projectName",
                table: "ProjectHealth",
                newName: "ProjectName");

            migrationBuilder.RenameColumn(
                name: "lastTransactionUpdate",
                table: "FinanceManager",
                newName: "LastTransactionUpdate");

            migrationBuilder.RenameColumn(
                name: "lastAPIRefreshStatusCode",
                table: "FinanceManager",
                newName: "LastAPIRefreshStatusCode");

            migrationBuilder.RenameColumn(
                name: "lastAPIRefresh",
                table: "FinanceManager",
                newName: "LastAPIRefresh");

            migrationBuilder.RenameColumn(
                name: "mode",
                table: "FinanceManager",
                newName: "Mode");

            migrationBuilder.RenameColumn(
                name: "resolvedAlertSent",
                table: "Errors",
                newName: "ResolvedAlertSent");

            migrationBuilder.RenameColumn(
                name: "projectName",
                table: "Errors",
                newName: "ProjectName");

            migrationBuilder.RenameColumn(
                name: "errorType",
                table: "Errors",
                newName: "ErrorType");

            migrationBuilder.RenameColumn(
                name: "errorDescription",
                table: "Errors",
                newName: "ErrorDescription");

            migrationBuilder.RenameColumn(
                name: "downtimeDuration",
                table: "Errors",
                newName: "DowntimeDuration");

            migrationBuilder.RenameColumn(
                name: "dateStarted",
                table: "Errors",
                newName: "DateStarted");

            migrationBuilder.RenameColumn(
                name: "dateEnded",
                table: "Errors",
                newName: "DateEnded");

            migrationBuilder.RenameColumn(
                name: "alertSent",
                table: "Errors",
                newName: "AlertSent");

            migrationBuilder.RenameColumn(
                name: "errorId",
                table: "Errors",
                newName: "ErrorId");

            migrationBuilder.RenameColumn(
                name: "lastTweet",
                table: "CatBot",
                newName: "LastTweet");

            migrationBuilder.RenameColumn(
                name: "lastDiscordPost",
                table: "CatBot",
                newName: "LastDiscordPost");

            migrationBuilder.RenameColumn(
                name: "discordConnectionStatus",
                table: "CatBot",
                newName: "DiscordConnectionStatus");

            migrationBuilder.RenameColumn(
                name: "mode",
                table: "CatBot",
                newName: "Mode");

            migrationBuilder.RenameColumn(
                name: "usersInStream",
                table: "BreganTwitchBot",
                newName: "UsersInStream");

            migrationBuilder.RenameColumn(
                name: "twitchPubSubConnectionStatus",
                table: "BreganTwitchBot",
                newName: "TwitchPubSubConnectionStatus");

            migrationBuilder.RenameColumn(
                name: "twitchIRCConnectionStatus",
                table: "BreganTwitchBot",
                newName: "TwitchIRCConnectionStatus");

            migrationBuilder.RenameColumn(
                name: "twitchApiKeyLastRefreshTime",
                table: "BreganTwitchBot",
                newName: "TwitchApiKeyLastRefreshTime");

            migrationBuilder.RenameColumn(
                name: "streamUptime",
                table: "BreganTwitchBot",
                newName: "StreamUptime");

            migrationBuilder.RenameColumn(
                name: "streamStatus",
                table: "BreganTwitchBot",
                newName: "StreamStatus");

            migrationBuilder.RenameColumn(
                name: "streamLiveTime",
                table: "BreganTwitchBot",
                newName: "StreamLiveTime");

            migrationBuilder.RenameColumn(
                name: "streamAnnounced",
                table: "BreganTwitchBot",
                newName: "StreamAnnounced");

            migrationBuilder.RenameColumn(
                name: "lastHoursUpdate",
                table: "BreganTwitchBot",
                newName: "LastHoursUpdate");

            migrationBuilder.RenameColumn(
                name: "lastDiscordLeaderboardsUpdate",
                table: "BreganTwitchBot",
                newName: "LastDiscordLeaderboardsUpdate");

            migrationBuilder.RenameColumn(
                name: "discordConnectionStatus",
                table: "BreganTwitchBot",
                newName: "DiscordConnectionStatus");

            migrationBuilder.RenameColumn(
                name: "dailyPointsEnabled",
                table: "BreganTwitchBot",
                newName: "DailyPointsEnabled");

            migrationBuilder.RenameColumn(
                name: "mode",
                table: "BreganTwitchBot",
                newName: "Mode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SystemUptime",
                table: "SystemHealth",
                newName: "systemUptime");

            migrationBuilder.RenameColumn(
                name: "SystemRunning",
                table: "SystemHealth",
                newName: "systemRunning");

            migrationBuilder.RenameColumn(
                name: "LastUpdate",
                table: "SystemHealth",
                newName: "lastUpdate");

            migrationBuilder.RenameColumn(
                name: "SystemName",
                table: "SystemHealth",
                newName: "systemName");

            migrationBuilder.RenameColumn(
                name: "TotalUsers",
                table: "RetroAchievementsTracker",
                newName: "totalUsers");

            migrationBuilder.RenameColumn(
                name: "TotalGames",
                table: "RetroAchievementsTracker",
                newName: "totalGames");

            migrationBuilder.RenameColumn(
                name: "LastGameUpdate",
                table: "RetroAchievementsTracker",
                newName: "lastGameUpdate");

            migrationBuilder.RenameColumn(
                name: "GamesUpdated",
                table: "RetroAchievementsTracker",
                newName: "gamesUpdated");

            migrationBuilder.RenameColumn(
                name: "Mode",
                table: "RetroAchievementsTracker",
                newName: "mode");

            migrationBuilder.RenameColumn(
                name: "RAMUsage",
                table: "ProjectHealth",
                newName: "ramUsage");

            migrationBuilder.RenameColumn(
                name: "ProjectUptime",
                table: "ProjectHealth",
                newName: "projectUptime");

            migrationBuilder.RenameColumn(
                name: "ProjectRunning",
                table: "ProjectHealth",
                newName: "projectRunning");

            migrationBuilder.RenameColumn(
                name: "LastUpdate",
                table: "ProjectHealth",
                newName: "lastUpdate");

            migrationBuilder.RenameColumn(
                name: "CPUUsage",
                table: "ProjectHealth",
                newName: "cpuUsage");

            migrationBuilder.RenameColumn(
                name: "ProjectName",
                table: "ProjectHealth",
                newName: "projectName");

            migrationBuilder.RenameColumn(
                name: "LastTransactionUpdate",
                table: "FinanceManager",
                newName: "lastTransactionUpdate");

            migrationBuilder.RenameColumn(
                name: "LastAPIRefreshStatusCode",
                table: "FinanceManager",
                newName: "lastAPIRefreshStatusCode");

            migrationBuilder.RenameColumn(
                name: "LastAPIRefresh",
                table: "FinanceManager",
                newName: "lastAPIRefresh");

            migrationBuilder.RenameColumn(
                name: "Mode",
                table: "FinanceManager",
                newName: "mode");

            migrationBuilder.RenameColumn(
                name: "ResolvedAlertSent",
                table: "Errors",
                newName: "resolvedAlertSent");

            migrationBuilder.RenameColumn(
                name: "ProjectName",
                table: "Errors",
                newName: "projectName");

            migrationBuilder.RenameColumn(
                name: "ErrorType",
                table: "Errors",
                newName: "errorType");

            migrationBuilder.RenameColumn(
                name: "ErrorDescription",
                table: "Errors",
                newName: "errorDescription");

            migrationBuilder.RenameColumn(
                name: "DowntimeDuration",
                table: "Errors",
                newName: "downtimeDuration");

            migrationBuilder.RenameColumn(
                name: "DateStarted",
                table: "Errors",
                newName: "dateStarted");

            migrationBuilder.RenameColumn(
                name: "DateEnded",
                table: "Errors",
                newName: "dateEnded");

            migrationBuilder.RenameColumn(
                name: "AlertSent",
                table: "Errors",
                newName: "alertSent");

            migrationBuilder.RenameColumn(
                name: "ErrorId",
                table: "Errors",
                newName: "errorId");

            migrationBuilder.RenameColumn(
                name: "LastTweet",
                table: "CatBot",
                newName: "lastTweet");

            migrationBuilder.RenameColumn(
                name: "LastDiscordPost",
                table: "CatBot",
                newName: "lastDiscordPost");

            migrationBuilder.RenameColumn(
                name: "DiscordConnectionStatus",
                table: "CatBot",
                newName: "discordConnectionStatus");

            migrationBuilder.RenameColumn(
                name: "Mode",
                table: "CatBot",
                newName: "mode");

            migrationBuilder.RenameColumn(
                name: "UsersInStream",
                table: "BreganTwitchBot",
                newName: "usersInStream");

            migrationBuilder.RenameColumn(
                name: "TwitchPubSubConnectionStatus",
                table: "BreganTwitchBot",
                newName: "twitchPubSubConnectionStatus");

            migrationBuilder.RenameColumn(
                name: "TwitchIRCConnectionStatus",
                table: "BreganTwitchBot",
                newName: "twitchIRCConnectionStatus");

            migrationBuilder.RenameColumn(
                name: "TwitchApiKeyLastRefreshTime",
                table: "BreganTwitchBot",
                newName: "twitchApiKeyLastRefreshTime");

            migrationBuilder.RenameColumn(
                name: "StreamUptime",
                table: "BreganTwitchBot",
                newName: "streamUptime");

            migrationBuilder.RenameColumn(
                name: "StreamStatus",
                table: "BreganTwitchBot",
                newName: "streamStatus");

            migrationBuilder.RenameColumn(
                name: "StreamLiveTime",
                table: "BreganTwitchBot",
                newName: "streamLiveTime");

            migrationBuilder.RenameColumn(
                name: "StreamAnnounced",
                table: "BreganTwitchBot",
                newName: "streamAnnounced");

            migrationBuilder.RenameColumn(
                name: "LastHoursUpdate",
                table: "BreganTwitchBot",
                newName: "lastHoursUpdate");

            migrationBuilder.RenameColumn(
                name: "LastDiscordLeaderboardsUpdate",
                table: "BreganTwitchBot",
                newName: "lastDiscordLeaderboardsUpdate");

            migrationBuilder.RenameColumn(
                name: "DiscordConnectionStatus",
                table: "BreganTwitchBot",
                newName: "discordConnectionStatus");

            migrationBuilder.RenameColumn(
                name: "DailyPointsEnabled",
                table: "BreganTwitchBot",
                newName: "dailyPointsEnabled");

            migrationBuilder.RenameColumn(
                name: "Mode",
                table: "BreganTwitchBot",
                newName: "mode");
        }
    }
}
