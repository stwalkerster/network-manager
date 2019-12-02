using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DnsWebApp.Migrations
{
    public partial class Owners : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Zones",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FavouriteDomains",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(nullable: true),
                    ZoneId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteDomains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavouriteDomains_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FavouriteDomains_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Zones_OwnerId",
                table: "Zones",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteDomains_UserId",
                table: "FavouriteDomains",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteDomains_ZoneId",
                table: "FavouriteDomains",
                column: "ZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Zones_AspNetUsers_OwnerId",
                table: "Zones",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zones_AspNetUsers_OwnerId",
                table: "Zones");

            migrationBuilder.DropTable(
                name: "FavouriteDomains");

            migrationBuilder.DropIndex(
                name: "IX_Zones_OwnerId",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Zones");
        }
    }
}
