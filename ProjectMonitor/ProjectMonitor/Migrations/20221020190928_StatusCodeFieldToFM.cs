using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class StatusCodeFieldToFM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "lastAPIRefreshStatusCode",
                table: "FinanceManager",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "twitchApiKeyLastRefreshTime" },
                values: new object[] { new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2587), new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2589), new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2593) });

            migrationBuilder.UpdateData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "twitchApiKeyLastRefreshTime" },
                values: new object[] { new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2595), new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2595), new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2596) });

            migrationBuilder.UpdateData(
                table: "CatBot",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastDiscordPost", "lastTweet" },
                values: new object[] { new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2742), new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2742) });

            migrationBuilder.UpdateData(
                table: "CatBot",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastDiscordPost", "lastTweet" },
                values: new object[] { new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2743), new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2743) });

            migrationBuilder.UpdateData(
                table: "FinanceManager",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastAPIRefresh", "lastAPIRefreshStatusCode", "lastTransactionUpdate" },
                values: new object[] { new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2694), "Success", new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2694) });

            migrationBuilder.UpdateData(
                table: "FinanceManager",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastAPIRefresh", "lastAPIRefreshStatusCode", "lastTransactionUpdate" },
                values: new object[] { new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2695), "Success", new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2695) });

            migrationBuilder.UpdateData(
                table: "RetroAchievementsTracker",
                keyColumn: "mode",
                keyValue: "debug",
                column: "lastGameUpdate",
                value: new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2679));

            migrationBuilder.UpdateData(
                table: "RetroAchievementsTracker",
                keyColumn: "mode",
                keyValue: "release",
                column: "lastGameUpdate",
                value: new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2681));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lastAPIRefreshStatusCode",
                table: "FinanceManager");

            migrationBuilder.UpdateData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "twitchApiKeyLastRefreshTime" },
                values: new object[] { new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(355), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(356), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(359) });

            migrationBuilder.UpdateData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "twitchApiKeyLastRefreshTime" },
                values: new object[] { new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(361), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(361), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(361) });

            migrationBuilder.UpdateData(
                table: "CatBot",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastDiscordPost", "lastTweet" },
                values: new object[] { new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(478), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(479) });

            migrationBuilder.UpdateData(
                table: "CatBot",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastDiscordPost", "lastTweet" },
                values: new object[] { new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(480), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(481) });

            migrationBuilder.UpdateData(
                table: "FinanceManager",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastAPIRefresh", "lastTransactionUpdate" },
                values: new object[] { new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(466), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(466) });

            migrationBuilder.UpdateData(
                table: "FinanceManager",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastAPIRefresh", "lastTransactionUpdate" },
                values: new object[] { new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(467), new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(467) });

            migrationBuilder.UpdateData(
                table: "RetroAchievementsTracker",
                keyColumn: "mode",
                keyValue: "debug",
                column: "lastGameUpdate",
                value: new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(446));

            migrationBuilder.UpdateData(
                table: "RetroAchievementsTracker",
                keyColumn: "mode",
                keyValue: "release",
                column: "lastGameUpdate",
                value: new DateTime(2022, 10, 4, 20, 49, 49, 509, DateTimeKind.Utc).AddTicks(447));
        }
    }
}