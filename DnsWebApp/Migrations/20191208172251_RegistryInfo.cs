using Microsoft.EntityFrameworkCore.Migrations;

namespace DnsWebApp.Migrations
{
    public partial class RegistryInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Registry",
                table: "TopLevelDomains",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistryUrl",
                table: "TopLevelDomains",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Registry",
                table: "TopLevelDomains");

            migrationBuilder.DropColumn(
                name: "RegistryUrl",
                table: "TopLevelDomains");
        }
    }
}
