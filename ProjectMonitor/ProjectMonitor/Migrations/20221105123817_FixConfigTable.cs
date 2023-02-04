using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class FixConfigTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "Config",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FromEmailAddress",
                table: "Config",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FromEmailAddressName",
                table: "Config",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HFConnectionString",
                table: "Config",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PMErrorsResolvedTemplateId",
                table: "Config",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PMErrorsTemplateId",
                table: "Config",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToEmailAddress",
                table: "Config",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToEmailAddressName",
                table: "Config",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "Config");

            migrationBuilder.DropColumn(
                name: "FromEmailAddress",
                table: "Config");

            migrationBuilder.DropColumn(
                name: "FromEmailAddressName",
                table: "Config");

            migrationBuilder.DropColumn(
                name: "HFConnectionString",
                table: "Config");

            migrationBuilder.DropColumn(
                name: "PMErrorsResolvedTemplateId",
                table: "Config");

            migrationBuilder.DropColumn(
                name: "PMErrorsTemplateId",
                table: "Config");

            migrationBuilder.DropColumn(
                name: "ToEmailAddress",
                table: "Config");

            migrationBuilder.DropColumn(
                name: "ToEmailAddressName",
                table: "Config");
        }
    }
}
