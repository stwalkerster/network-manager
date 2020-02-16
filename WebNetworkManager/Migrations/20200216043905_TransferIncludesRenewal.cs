using Microsoft.EntityFrameworkCore.Migrations;

namespace DnsWebApp.Migrations
{
    public partial class TransferIncludesRenewal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TransferIncludesRenewal",
                table: "RegistrarTldSupport",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransferIncludesRenewal",
                table: "RegistrarTldSupport");
        }
    }
}
