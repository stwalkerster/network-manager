using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DnsWebApp.Migrations
{
    public partial class Registrar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Zones",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "RegistrarId",
                table: "Zones",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Registrar",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrar", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Zones_RegistrarId",
                table: "Zones",
                column: "RegistrarId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrar_Name",
                table: "Registrar",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Zones_Registrar_RegistrarId",
                table: "Zones",
                column: "RegistrarId",
                principalTable: "Registrar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zones_Registrar_RegistrarId",
                table: "Zones");

            migrationBuilder.DropTable(
                name: "Registrar");

            migrationBuilder.DropIndex(
                name: "IX_Zones_RegistrarId",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "RegistrarId",
                table: "Zones");
        }
    }
}
