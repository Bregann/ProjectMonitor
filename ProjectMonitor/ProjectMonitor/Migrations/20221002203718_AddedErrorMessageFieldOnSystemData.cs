using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class AddedErrorMessageFieldOnSystemData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "errorMessageSent",
                table: "SystemHealth",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "errorMessageSent",
                table: "SystemHealth");
        }
    }
}
