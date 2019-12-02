using Microsoft.EntityFrameworkCore.Migrations;

namespace LdapDnsWebApp.Migrations
{
    public partial class TldNormalisation2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zones_TopLevelDomains_TopLevelDomainId",
                table: "Zones");

            migrationBuilder.AlterColumn<long>(
                name: "TopLevelDomainId",
                table: "Zones",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Zones_TopLevelDomains_TopLevelDomainId",
                table: "Zones",
                column: "TopLevelDomainId",
                principalTable: "TopLevelDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zones_TopLevelDomains_TopLevelDomainId",
                table: "Zones");

            migrationBuilder.AlterColumn<long>(
                name: "TopLevelDomainId",
                table: "Zones",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_Zones_TopLevelDomains_TopLevelDomainId",
                table: "Zones",
                column: "TopLevelDomainId",
                principalTable: "TopLevelDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
