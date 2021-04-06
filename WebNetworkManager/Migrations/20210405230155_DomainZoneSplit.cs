using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DnsWebApp.Migrations
{
    public partial class DomainZoneSplit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zones_Registrar_RegistrarId",
                table: "Zones");

            migrationBuilder.DropForeignKey(
                name: "FK_Zones_TopLevelDomains_TopLevelDomainId",
                table: "Zones");

            migrationBuilder.DropIndex(
                name: "IX_Zones_RegistrarId",
                table: "Zones");

            migrationBuilder.DropIndex(
                name: "IX_Zones_TopLevelDomainId_Name_HorizonViewId",
                table: "Zones");
            
            migrationBuilder.AddColumn<long>(
                name: "DomainId",
                table: "Zones",
                nullable: true);
            
            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TopLevelDomainId = table.Column<long>(type: "bigint", nullable: false),
                    RegistrationExpiry = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OwnerId = table.Column<string>(type: "text", nullable: true),
                    RegistrarId = table.Column<long>(type: "bigint", nullable: true),
                    WhoisLastUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Placeholder = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Domains_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Domains_Registrar_RegistrarId",
                        column: x => x.RegistrarId,
                        principalTable: "Registrar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Domains_TopLevelDomains_TopLevelDomainId",
                        column: x => x.TopLevelDomainId,
                        principalTable: "TopLevelDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                "INSERT INTO \"Domains\" (\"Name\", \"TopLevelDomainId\", \"RegistrationExpiry\", \"OwnerId\", \"RegistrarId\", \"WhoisLastUpdated\", \"LastUpdated\", \"Placeholder\")\nSELECT DISTINCT ON(\"Name\", \"TopLevelDomainId\") \"Name\", \"TopLevelDomainId\", \"RegistrationExpiry\", \"OwnerId\", \"RegistrarId\", \"WhoisLastUpdated\", \"LastUpdated\", false FROM \"Zones\";");

            migrationBuilder.Sql(
                "UPDATE \"Zones\" z SET \"DomainId\" = (SELECT \"Id\" FROM \"Domains\" d WHERE d.\"Name\" = z.\"Name\" AND d.\"TopLevelDomainId\" = z.\"TopLevelDomainId\") WHERE 1=1;");

            migrationBuilder.AlterColumn<long>(
                name: "DomainId",
                table: "Zones",
                nullable: false,
                oldNullable: true);
            
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "RegistrarId",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "RegistrationExpiry",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "WhoisLastUpdated",
                table: "Zones");
            
            migrationBuilder.DropColumn(
                name: "TopLevelDomainId",
                table: "Zones");
            
            migrationBuilder.CreateIndex(
                name: "IX_Zones_DomainId_HorizonViewId",
                table: "Zones",
                columns: new[] { "DomainId", "HorizonViewId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Domains_Name_TopLevelDomainId",
                table: "Domains",
                columns: new[] { "Name", "TopLevelDomainId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Domains_OwnerId",
                table: "Domains",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_RegistrarId",
                table: "Domains",
                column: "RegistrarId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_TopLevelDomainId",
                table: "Domains",
                column: "TopLevelDomainId");

            migrationBuilder.AddForeignKey(
                name: "FK_Zones_Domains_DomainId",
                table: "Zones",
                column: "DomainId",
                principalTable: "Domains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zones_Domains_DomainId",
                table: "Zones");

            migrationBuilder.DropIndex(
                name: "IX_Zones_DomainId_HorizonViewId",
                table: "Zones");
            
            migrationBuilder.AddColumn<long>(
                name: "TopLevelDomainId",
                table: "Zones",
                nullable: true);
            
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Zones",
                type: "text",
                nullable: true);
            
            migrationBuilder.AddColumn<long>(
                name: "RegistrarId",
                table: "Zones",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationExpiry",
                table: "Zones",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WhoisLastUpdated",
                table: "Zones",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Zones\" z SET \n    \"TopLevelDomainId\" = (SELECT d.\"TopLevelDomainId\" FROM \"Domains\" d WHERE d.\"Id\" = z.\"DomainId\"),\n    \"Name\" = (SELECT d.\"Name\" FROM \"Domains\" d WHERE d.\"Id\" = z.\"DomainId\"),\n    \"RegistrarId\" = (SELECT d.\"RegistrarId\" FROM \"Domains\" d WHERE d.\"Id\" = z.\"DomainId\"),\n    \"RegistrationExpiry\" = (SELECT d.\"RegistrationExpiry\" FROM \"Domains\" d WHERE d.\"Id\" = z.\"DomainId\"),\n    \"WhoisLastUpdated\" = (SELECT d.\"WhoisLastUpdated\" FROM \"Domains\" d WHERE d.\"Id\" = z.\"DomainId\")\nWHERE 1=1");
            
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Zones",
                type: "text",
                nullable: false,
                oldNullable: true);
            
            migrationBuilder.AlterColumn<long>(
                name: "TopLevelDomainId",
                table: "Zones",
                nullable: false,
                oldNullable: true);
            
            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropColumn("DomainId", "Zones");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_RegistrarId",
                table: "Zones",
                column: "RegistrarId");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_TopLevelDomainId_Name_HorizonViewId",
                table: "Zones",
                columns: new[] { "TopLevelDomainId", "Name", "HorizonViewId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Zones_Registrar_RegistrarId",
                table: "Zones",
                column: "RegistrarId",
                principalTable: "Registrar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Zones_TopLevelDomains_TopLevelDomainId",
                table: "Zones",
                column: "TopLevelDomainId",
                principalTable: "TopLevelDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
