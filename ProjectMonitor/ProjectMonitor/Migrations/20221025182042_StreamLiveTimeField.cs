using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class StreamLiveTimeField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "streamLiveTime",
                table: "BreganTwitchBot",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "streamLiveTime",
                table: "BreganTwitchBot");
        }
    }
}
