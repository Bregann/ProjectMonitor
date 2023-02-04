using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class AddTextColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResolvedAlertSent",
                table: "Errors",
                newName: "TextResolvedAlertSent");

            migrationBuilder.RenameColumn(
                name: "AlertSent",
                table: "Errors",
                newName: "TextAlertSent");

            migrationBuilder.AddColumn<bool>(
                name: "EmailAlertSent",
                table: "Errors",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmailResolvedAlertSent",
                table: "Errors",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ChatId",
                table: "Config",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAlertSent",
                table: "Errors");

            migrationBuilder.DropColumn(
                name: "EmailResolvedAlertSent",
                table: "Errors");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Config");

            migrationBuilder.RenameColumn(
                name: "TextResolvedAlertSent",
                table: "Errors",
                newName: "ResolvedAlertSent");

            migrationBuilder.RenameColumn(
                name: "TextAlertSent",
                table: "Errors",
                newName: "AlertSent");
        }
    }
}
