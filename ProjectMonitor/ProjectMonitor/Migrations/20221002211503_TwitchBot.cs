using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class TwitchBot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BreganTwitchBot",
                columns: table => new
                {
                    mode = table.Column<string>(type: "text", nullable: false),
                    usersInStream = table.Column<long>(type: "bigint", nullable: false),
                    twitchIRCConnectionStatus = table.Column<bool>(type: "boolean", nullable: false),
                    twitchPubSubConnectionStatus = table.Column<bool>(type: "boolean", nullable: false),
                    twitchApiKeyLastRefreshTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    discordConnectionStatus = table.Column<bool>(type: "boolean", nullable: false),
                    streamAnnounced = table.Column<bool>(type: "boolean", nullable: false),
                    streamStatus = table.Column<bool>(type: "boolean", nullable: false),
                    streamUptime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    dailyPointsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    lastDiscordLeaderboardsUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastHoursUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreganTwitchBot", x => x.mode);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BreganTwitchBot");
        }
    }
}