using Microsoft.EntityFrameworkCore.Migrations;

namespace DnsWebApp.Migrations
{
    public partial class ZoneGroupOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "ZoneGroups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ZoneGroups_OwnerId",
                table: "ZoneGroups",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ZoneGroups_AspNetUsers_OwnerId",
                table: "ZoneGroups",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ZoneGroups_AspNetUsers_OwnerId",
                table: "ZoneGroups");

            migrationBuilder.DropIndex(
                name: "IX_ZoneGroups_OwnerId",
                table: "ZoneGroups");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "ZoneGroups");
        }
    }
}
