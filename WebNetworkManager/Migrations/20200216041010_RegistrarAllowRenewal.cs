using Microsoft.EntityFrameworkCore.Migrations;

namespace DnsWebApp.Migrations
{
    public partial class RegistrarAllowRenewal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowRenewals",
                table: "Registrar",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowRenewals",
                table: "Registrar");
        }
    }
}
