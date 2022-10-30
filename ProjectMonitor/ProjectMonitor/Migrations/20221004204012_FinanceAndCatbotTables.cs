using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class FinanceAndCatbotTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatBot",
                columns: table => new
                {
                    mode = table.Column<string>(type: "text", nullable: false),
                    discordConnectionStatus = table.Column<bool>(type: "boolean", nullable: false),
                    lastTweet = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastDiscordPost = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatBot", x => x.mode);
                });

            migrationBuilder.CreateTable(
                name: "FinanceManager",
                columns: table => new
                {
                    mode = table.Column<string>(type: "text", nullable: false),
                    lastTransactionUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastAPIRefresh = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceManager", x => x.mode);
                });

            migrationBuilder.UpdateData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "twitchApiKeyLastRefreshTime" },
                values: new object[] { new DateTime(2022, 10, 4, 20, 40, 12, 623, DateTimeKind.Utc).AddTicks(1788), new DateTime(2022, 10, 4, 20, 40, 12, 623, DateTimeKind.Utc).AddTicks(1788), new DateTime(2022, 10, 4, 20, 40, 12, 623, DateTimeKind.Utc).AddTicks(1791) });

            migrationBuilder.UpdateData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "twitchApiKeyLastRefreshTime" },
                values: new object[] { new DateTime(2022, 10, 4, 20, 40, 12, 623, DateTimeKind.Utc).AddTicks(1793), new DateTime(2022, 10, 4, 20, 40, 12, 623, DateTimeKind.Utc).AddTicks(1793), new DateTime(2022, 10, 4, 20, 40, 12, 623, DateTimeKind.Utc).AddTicks(1794) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatBot");

            migrationBuilder.DropTable(
                name: "FinanceManager");

            migrationBuilder.UpdateData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "debug",
                columns: new[] { "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "twitchApiKeyLastRefreshTime" },
                values: new object[] { new DateTime(2022, 10, 4, 20, 31, 48, 788, DateTimeKind.Utc).AddTicks(5172), new DateTime(2022, 10, 4, 20, 31, 48, 788, DateTimeKind.Utc).AddTicks(5172), new DateTime(2022, 10, 4, 20, 31, 48, 788, DateTimeKind.Utc).AddTicks(5175) });

            migrationBuilder.UpdateData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "twitchApiKeyLastRefreshTime" },
                values: new object[] { new DateTime(2022, 10, 4, 20, 31, 48, 788, DateTimeKind.Utc).AddTicks(5177), new DateTime(2022, 10, 4, 20, 31, 48, 788, DateTimeKind.Utc).AddTicks(5177), new DateTime(2022, 10, 4, 20, 31, 48, 788, DateTimeKind.Utc).AddTicks(5178) });
        }
    }
}
