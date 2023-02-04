using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class RemoveSeedingTemp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "debug");

            migrationBuilder.DeleteData(
                table: "BreganTwitchBot",
                keyColumn: "mode",
                keyValue: "release");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BreganTwitchBot",
                columns: new[] { "mode", "dailyPointsEnabled", "discordConnectionStatus", "lastDiscordLeaderboardsUpdate", "lastHoursUpdate", "streamAnnounced", "streamStatus", "streamUptime", "twitchApiKeyLastRefreshTime", "twitchIRCConnectionStatus", "twitchPubSubConnectionStatus", "usersInStream" },
                values: new object[,]
                {
                    { "debug", false, true, new DateTime(2022, 10, 4, 20, 40, 12, 623, DateTimeKind.Utc).AddTicks(1788), new DateTime(2022, 10, 4, 20, 40, 12, 623, DateTimeKind.Utc).AddTicks(1788), false, false, new TimeSpan(0, 0, 0, 1, 0), new DateTime(2022, 10, 4, 20, 40, 12, 623, DateTimeKind.Utc).AddTicks(1791), true, true, 0L },
                    { "release", false, true, new DateTime(2022, 10, 4, 20, 40, 12, 623, DateTimeKind.Utc).AddTicks(1793), new DateTime(2022, 10, 4, 20, 40, 12, 623, DateTimeKind.Utc).AddTicks(1793), false, false, new TimeSpan(0, 0, 0, 1, 0), new DateTime(2022, 10, 4, 20, 40, 12, 623, DateTimeKind.Utc).AddTicks(1794), true, true, 0L }
                });
        }
    }
}