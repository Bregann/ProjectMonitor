using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSystemAndAddHFConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemHealth");

            migrationBuilder.AddColumn<string>(
                name: "HangfirePassword",
                table: "Config",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HangfireUsername",
                table: "Config",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Config",
                keyColumn: "RowId",
                keyValue: 1,
                columns: new[] { "HangfirePassword", "HangfireUsername" },
                values: new object[] { "", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HangfirePassword",
                table: "Config");

            migrationBuilder.DropColumn(
                name: "HangfireUsername",
                table: "Config");

            migrationBuilder.CreateTable(
                name: "SystemHealth",
                columns: table => new
                {
                    SystemName = table.Column<string>(type: "text", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SystemRunning = table.Column<bool>(type: "boolean", nullable: false),
                    SystemUptime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemHealth", x => x.SystemName);
                });
        }
    }
}
