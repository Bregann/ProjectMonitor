using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class UpdateModelCreating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "twitchApiKeyLastRefreshTime" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "twitchApiKeyLastRefreshTime" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CatBot",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastDiscordPost", "lastTweet" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "CatBot",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastDiscordPost", "lastTweet" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "FinanceManager",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastAPIRefresh", "lastTransactionUpdate" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "FinanceManager",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastAPIRefresh", "lastTransactionUpdate" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "RetroAchievementsTracker",
                keyColumn: "mode",
                keyValue: "debug",
                column: "lastGameUpdate",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "RetroAchievementsTracker",
                keyColumn: "mode",
                keyValue: "release",
                column: "lastGameUpdate",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "twitchApiKeyLastRefreshTime" },
                values: new object[] { new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8630), new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8631), new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8638) });

            migrationBuilder.UpdateData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "twitchApiKeyLastRefreshTime" },
                values: new object[] { new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8639), new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8640), new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8640) });

            migrationBuilder.UpdateData(
                table: "CatBot",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastDiscordPost", "lastTweet" },
                values: new object[] { new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8757), new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8758) });

            migrationBuilder.UpdateData(
                table: "CatBot",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastDiscordPost", "lastTweet" },
                values: new object[] { new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8759), new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8759) });

            migrationBuilder.UpdateData(
                table: "FinanceManager",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastAPIRefresh", "lastTransactionUpdate" },
                values: new object[] { new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8744), new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8744) });

            migrationBuilder.UpdateData(
                table: "FinanceManager",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastAPIRefresh", "lastTransactionUpdate" },
                values: new object[] { new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8746), new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8746) });

            migrationBuilder.UpdateData(
                table: "RetroAchievementsTracker",
                keyColumn: "mode",
                keyValue: "debug",
                column: "lastGameUpdate",
                value: new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8728));

            migrationBuilder.UpdateData(
                table: "RetroAchievementsTracker",
                keyColumn: "mode",
                keyValue: "release",
                column: "lastGameUpdate",
                value: new DateTime(2022, 10, 23, 19, 33, 3, 998, DateTimeKind.Utc).AddTicks(8730));
        }
    }
}