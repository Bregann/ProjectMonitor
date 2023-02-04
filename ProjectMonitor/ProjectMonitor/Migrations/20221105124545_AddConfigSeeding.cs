using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectMonitor.Api.Migrations
{
    public partial class AddConfigSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Config",
                columns: new[] { "RowId", "ApiKey", "FromEmailAddress", "FromEmailAddressName", "HFConnectionString", "PMErrorsResolvedTemplateId", "PMErrorsTemplateId", "ToEmailAddress", "ToEmailAddressName" },
                values: new object[] { 1, "", "", "", "", "", "", "", "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Config",
                keyColumn: "RowId",
                keyValue: 1);
        }
    }
}
