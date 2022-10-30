using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class AddRATable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RetroAchievementsTracker",
                columns: table => new
                {
                    mode = table.Column<string>(type: "text", nullable: false),
                    totalGames = table.Column<long>(type: "bigint", nullable: false),
                    totalUsers = table.Column<long>(type: "bigint", nullable: false),
                    lastGameUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    gamesUpdated = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetroAchievementsTracker", x => x.mode);
                });

            migrationBuilder.InsertData(
                table: "BreganTwitchBot",
                columns: new[] { "mode", "dailyPointsEnabled", "discordConnectionStatus", "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "streamAnnounced", "streamStatus", "streamUptime", "twitchApiKeyLastRefreshTime", "twitchIRCConnectionStatus", "twitchPubSubConnectionStatus", "usersInStream" },
                values: new object[,]
                {
                    { "debug", false, true, new DateTime(2022, 10, 4, 20, 31, 48, 788, DateTimeKind.Utc).AddTicks(5172), new DateTime(2022, 10, 4, 20, 31, 48, 788, DateTimeKind.Utc).AddTicks(5172), false, false, new TimeSpan(0, 0, 0, 1, 0), new DateTime(2022, 10, 4, 20, 31, 48, 788, DateTimeKind.Utc).AddTicks(5175), true, true, 0L },
                    { "release", false, true, new DateTime(2022, 10, 4, 20, 31, 48, 788, DateTimeKind.Utc).AddTicks(5177), new DateTime(2022, 10, 4, 20, 31, 48, 788, DateTimeKind.Utc).AddTicks(5177), false, false, new TimeSpan(0, 0, 0, 1, 0), new DateTime(2022, 10, 4, 20, 31, 48, 788, DateTimeKind.Utc).AddTicks(5178), true, true, 0L }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetroAchievementsTracker");

            migrationBuilder.DeleteData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "debug");

            migrationBuilder.DeleteData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "release");
        }
    }
}
