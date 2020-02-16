using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DnsWebApp.Migrations
{
    public partial class Currency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Zones_TopLevelDomainId",
                table: "Zones");

            migrationBuilder.DropIndex(
                name: "IX_RegistrarTldSupport_RegistrarId",
                table: "RegistrarTldSupport");

            migrationBuilder.AddColumn<DateTime>(
                name: "RenewalPriceUpdated",
                table: "RegistrarTldSupport",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "Registrar",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PricesIncludeVat",
                table: "Registrar",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Symbol = table.Column<string>(nullable: true),
                    ExchangeRate = table.Column<decimal>(nullable: true),
                    ExchangeRateUpdated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Zones_TopLevelDomainId_Name_HorizonViewId",
                table: "Zones",
                columns: new[] { "TopLevelDomainId", "Name", "HorizonViewId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTldSupport_RegistrarId_TopLevelDomainId",
                table: "RegistrarTldSupport",
                columns: new[] { "RegistrarId", "TopLevelDomainId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrar_CurrencyId",
                table: "Registrar",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrar_Currencies_CurrencyId",
                table: "Registrar",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registrar_Currencies_CurrencyId",
                table: "Registrar");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Zones_TopLevelDomainId_Name_HorizonViewId",
                table: "Zones");

            migrationBuilder.DropIndex(
                name: "IX_RegistrarTldSupport_RegistrarId_TopLevelDomainId",
                table: "RegistrarTldSupport");

            migrationBuilder.DropIndex(
                name: "IX_Registrar_CurrencyId",
                table: "Registrar");

            migrationBuilder.DropColumn(
                name: "RenewalPriceUpdated",
                table: "RegistrarTldSupport");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Registrar");

            migrationBuilder.DropColumn(
                name: "PricesIncludeVat",
                table: "Registrar");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_TopLevelDomainId",
                table: "Zones",
                column: "TopLevelDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTldSupport_RegistrarId",
                table: "RegistrarTldSupport",
                column: "RegistrarId");
        }
    }
}
