using Microsoft.EntityFrameworkCore.Migrations;

namespace LdapDnsWebApp.Migrations
{
    public partial class TldNormalisation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Zones_Name",
                table: "Zones");

            migrationBuilder.AddColumn<long>(
                name: "TopLevelDomainId",
                table: "Zones",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zones_TopLevelDomainId",
                table: "Zones",
                column: "TopLevelDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_TopLevelDomains_Domain",
                table: "TopLevelDomains",
                column: "Domain",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Zones_TopLevelDomains_TopLevelDomainId",
                table: "Zones",
                column: "TopLevelDomainId",
                principalTable: "TopLevelDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(
                "update \"Zones\" z set \"TopLevelDomainId\" = tld.\"Id\", \"Name\" = regexp_replace(z.\"Name\", '^([^.]+).*', '\\1') From \"TopLevelDomains\" tld WHERE z.\"Name\" = regexp_replace(z.\"Name\", '^([^.]+).*', '\\1') || '.' || tld.\"Domain\";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "update \"Zones\" z set \"Name\" = \"Name\" || '.' || tld.\"Domain\" From \"TopLevelDomains\" tld WHERE z.\"TopLevelDomainId\" = tld.\"Id\";");
            
            migrationBuilder.DropForeignKey(
                name: "FK_Zones_TopLevelDomains_TopLevelDomainId",
                table: "Zones");

            migrationBuilder.DropIndex(
                name: "IX_Zones_TopLevelDomainId",
                table: "Zones");

            migrationBuilder.DropIndex(
                name: "IX_TopLevelDomains_Domain",
                table: "TopLevelDomains");

            migrationBuilder.DropColumn(
                name: "TopLevelDomainId",
                table: "Zones");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_Name",
                table: "Zones",
                column: "Name",
                unique: true);
        }
    }
}
