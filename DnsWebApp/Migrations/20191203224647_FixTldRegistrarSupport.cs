using Microsoft.EntityFrameworkCore.Migrations;

namespace DnsWebApp.Migrations
{
    public partial class FixTldRegistrarSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrarTldSupport_Registrar_RegistrarId",
                table: "RegistrarTldSupport");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrarTldSupport_TopLevelDomains_TopLevelDomainId",
                table: "RegistrarTldSupport");

            migrationBuilder.AlterColumn<long>(
                name: "TopLevelDomainId",
                table: "RegistrarTldSupport",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RenewalPrice",
                table: "RegistrarTldSupport",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<long>(
                name: "RegistrarId",
                table: "RegistrarTldSupport",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrarTldSupport_Registrar_RegistrarId",
                table: "RegistrarTldSupport",
                column: "RegistrarId",
                principalTable: "Registrar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrarTldSupport_TopLevelDomains_TopLevelDomainId",
                table: "RegistrarTldSupport",
                column: "TopLevelDomainId",
                principalTable: "TopLevelDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrarTldSupport_Registrar_RegistrarId",
                table: "RegistrarTldSupport");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrarTldSupport_TopLevelDomains_TopLevelDomainId",
                table: "RegistrarTldSupport");

            migrationBuilder.AlterColumn<long>(
                name: "TopLevelDomainId",
                table: "RegistrarTldSupport",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<decimal>(
                name: "RenewalPrice",
                table: "RegistrarTldSupport",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "RegistrarId",
                table: "RegistrarTldSupport",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrarTldSupport_Registrar_RegistrarId",
                table: "RegistrarTldSupport",
                column: "RegistrarId",
                principalTable: "Registrar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrarTldSupport_TopLevelDomains_TopLevelDomainId",
                table: "RegistrarTldSupport",
                column: "TopLevelDomainId",
                principalTable: "TopLevelDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
