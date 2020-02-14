using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DnsWebApp.Migrations
{
    public partial class TldSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationExpiry",
                table: "Zones",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TopLevelDomains",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Domain = table.Column<string>(nullable: false),
                    WhoisServer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopLevelDomains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrarTldSupport",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistrarId = table.Column<long>(nullable: true),
                    TopLevelDomainId = table.Column<long>(nullable: true),
                    RenewalPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrarTldSupport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrarTldSupport_Registrar_RegistrarId",
                        column: x => x.RegistrarId,
                        principalTable: "Registrar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrarTldSupport_TopLevelDomains_TopLevelDomainId",
                        column: x => x.TopLevelDomainId,
                        principalTable: "TopLevelDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTldSupport_RegistrarId",
                table: "RegistrarTldSupport",
                column: "RegistrarId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTldSupport_TopLevelDomainId",
                table: "RegistrarTldSupport",
                column: "TopLevelDomainId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrarTldSupport");

            migrationBuilder.DropTable(
                name: "TopLevelDomains");

            migrationBuilder.DropColumn(
                name: "RegistrationExpiry",
                table: "Zones");
        }
    }
}
