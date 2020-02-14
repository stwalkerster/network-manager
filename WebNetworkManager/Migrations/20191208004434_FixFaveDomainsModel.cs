using Microsoft.EntityFrameworkCore.Migrations;

namespace DnsWebApp.Migrations
{
    public partial class FixFaveDomainsModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavouriteDomains_AspNetUsers_UserId",
                table: "FavouriteDomains");

            migrationBuilder.DropForeignKey(
                name: "FK_FavouriteDomains_Zones_ZoneId",
                table: "FavouriteDomains");

            migrationBuilder.AlterColumn<long>(
                name: "ZoneId",
                table: "FavouriteDomains",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "FavouriteDomains",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FavouriteDomains_AspNetUsers_UserId",
                table: "FavouriteDomains",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavouriteDomains_Zones_ZoneId",
                table: "FavouriteDomains",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavouriteDomains_AspNetUsers_UserId",
                table: "FavouriteDomains");

            migrationBuilder.DropForeignKey(
                name: "FK_FavouriteDomains_Zones_ZoneId",
                table: "FavouriteDomains");

            migrationBuilder.AlterColumn<long>(
                name: "ZoneId",
                table: "FavouriteDomains",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "FavouriteDomains",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_FavouriteDomains_AspNetUsers_UserId",
                table: "FavouriteDomains",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FavouriteDomains_Zones_ZoneId",
                table: "FavouriteDomains",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
