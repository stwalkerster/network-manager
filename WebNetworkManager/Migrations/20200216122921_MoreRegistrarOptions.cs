using Microsoft.EntityFrameworkCore.Migrations;

namespace DnsWebApp.Migrations
{
    public partial class MoreRegistrarOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PrivacyIncluded",
                table: "RegistrarTldSupport",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowTransfers",
                table: "Registrar",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PrivacyFee",
                table: "Registrar",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TransferOutFee",
                table: "Registrar",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrivacyIncluded",
                table: "RegistrarTldSupport");

            migrationBuilder.DropColumn(
                name: "AllowTransfers",
                table: "Registrar");

            migrationBuilder.DropColumn(
                name: "PrivacyFee",
                table: "Registrar");

            migrationBuilder.DropColumn(
                name: "TransferOutFee",
                table: "Registrar");
        }
    }
}
