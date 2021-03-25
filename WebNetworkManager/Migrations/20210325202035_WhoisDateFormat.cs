using Microsoft.EntityFrameworkCore.Migrations;

namespace DnsWebApp.Migrations
{
    public partial class WhoisDateFormat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WhoisExpiryDateFormat",
                table: "TopLevelDomains",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WhoisExpiryDateFormat",
                table: "TopLevelDomains");
        }
    }
}
