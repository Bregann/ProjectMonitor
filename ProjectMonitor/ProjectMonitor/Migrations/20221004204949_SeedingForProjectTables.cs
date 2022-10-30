using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class SeedingForProjectTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BreganTwitchBot",
                columns: new[] { "mode", "dailyPointsEnabled", "discordConnectionStatus", "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "streamAnnounced", "streamStatus", "streamUptime", "twitchApiKeyLastRefreshTime", "twitchIRCConnectionStatus", "twitchPubSubConnectionStatus", "usersInStream" },
                values: new object[,]
                {
                    { "debug", false, true, new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(355), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(356), false, false, new TimeSpan(0, 0, 0, 0, 0), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(359), true, true, 0L },
                    { "release", false, true, new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(361), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(361), false, false, new TimeSpan(0, 0, 0, 0, 0), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(361), true, true, 0L }
                });

            migrationBuilder.InsertData(
                table: "CatBot",
                columns: new[] { "mode", "discordConnectionStatus", "lastDiscordPost", "lastTweet" },
                values: new object[,]
                {
                    { "debug", true, new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(478), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(479) },
                    { "release", true, new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(480), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(481) }
                });

            migrationBuilder.InsertData(
                table: "FinanceManager",
                columns: new[] { "mode", "lastAPIRefresh", "lastTransactionUpdate" },
                values: new object[,]
                {
                    { "debug", new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(466), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(466) },
                    { "release", new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(467), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(467) }
                });

            migrationBuilder.InsertData(
                table: "RetroAchievementsTracker",
                columns: new[] { "mode", "gamesUpdated", "lastGameUpdate", "totalGames", "totalUsers" },
                values: new object[,]
                {
                    { "debug", 0L, new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(446), 0L, 0L },
                    { "release", 0L, new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(447), 0L, 0L }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "debug");

            migrationBuilder.DeleteData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "release");

            migrationBuilder.DeleteData(
                table: "CatBot",
                keyColumn: "mode",
                keyValue: "debug");

            migrationBuilder.DeleteData(
                table: "CatBot",
                keyColumn: "mode",
                keyValue: "release");

            migrationBuilder.DeleteData(
                table: "FinanceManager",
                keyColumn: "mode",
                keyValue: "debug");

            migrationBuilder.DeleteData(
                table: "FinanceManager",
                keyColumn: "mode",
                keyValue: "release");

            migrationBuilder.DeleteData(
                table: "RetroAchievementsTracker",
                keyColumn: "mode",
                keyValue: "debug");

            migrationBuilder.DeleteData(
                table: "RetroAchievementsTracker",
                keyColumn: "mode",
                keyValue: "release");
        }
    }
}
