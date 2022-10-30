using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class AddErrorsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "errorMessageSent",
                table: "SystemHealth");

            migrationBuilder.DropColumn(
                name: "errorMessageSent",
                table: "ProjectHealth");

            migrationBuilder.CreateTable(
                name: "Errors",
                columns: table => new
                {
                    errorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dateStarted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dateEnded = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    downtimeDuration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    projectName = table.Column<string>(type: "text", nullable: false),
                    errorType = table.Column<string>(type: "text", nullable: false),
                    errorDescription = table.Column<string>(type: "text", nullable: false),
                    alertSent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Errors", x => x.errorId);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Errors");

            migrationBuilder.AddColumn<bool>(
                name: "errorMessageSent",
                table: "SystemHealth",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "errorMessageSent",
                table: "ProjectHealth",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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
                columns: new[] { "lastAPIRefresh", "lastTransactionUpdate" },
                values: new object[] { new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2694), new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2694) });

            migrationBuilder.UpdateData(
                table: "FinanceManager",
                keyColumn: "mode",
                keyValue: "release",
                columns: new[] { "lastAPIRefresh", "lastTransactionUpdate" },
                values: new object[] { new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2695), new DateTime(2022, 10, 20, 19, 9, 28, 385, DateTimeKind.Utc).AddTicks(2695) });

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
    }
}
