using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DnsWebApp.Migrations
{
    public partial class HorizonViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "HorizonViewId",
                table: "Zones",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HorizonViews",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ViewName = table.Column<string>(nullable: false),
                    ViewTag = table.Column<string>(nullable: true),
                    ViewTagColour = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorizonViews", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Zones_HorizonViewId",
                table: "Zones",
                column: "HorizonViewId");

            migrationBuilder.AddForeignKey(
                name: "FK_Zones_HorizonViews_HorizonViewId",
                table: "Zones",
                column: "HorizonViewId",
                principalTable: "HorizonViews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zones_HorizonViews_HorizonViewId",
                table: "Zones");

            migrationBuilder.DropTable(
                name: "HorizonViews");

            migrationBuilder.DropIndex(
                name: "IX_Zones_HorizonViewId",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "HorizonViewId",
                table: "Zones");
        }
    }
}
