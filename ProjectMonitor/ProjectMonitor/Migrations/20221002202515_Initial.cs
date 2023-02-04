using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectHealth",
                columns: table => new
                {
                    projectName = table.Column<string>(type: "text", nullable: false),
                    cpuUsage = table.Column<double>(type: "double precision", nullable: false),
                    ramUsage = table.Column<long>(type: "bigint", nullable: false),
                    projectUptime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    lastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    projectRunning = table.Column<bool>(type: "boolean", nullable: false),
                    errorMessageSent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectHealth", x => x.projectName);
                });

            migrationBuilder.CreateTable(
                name: "SystemHealth",
                columns: table => new
                {
                    systemName = table.Column<string>(type: "text", nullable: false),
                    systemUptime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    lastUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemHealth", x => x.systemName);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectHealth");

            migrationBuilder.DropTable(
                name: "SystemHealth");
        }
    }
}