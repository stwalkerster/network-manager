using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DnsWebApp.Migrations
{
    public partial class txtrecordquotemigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"Zones\" SET \"SerialNumber\" = \"SerialNumber\" + 1 WHERE \"Id\" IN (\nSELECT zgm.\"ZoneId\" FROM \"ZoneGroupMember\" zgm WHERE zgm.\"ZoneGroupId\" IN (\nSELECT rzg.\"ZoneGroupId\" \nFROM \"Record\" rzg\nWHERE (rzg.\"Value\" LIKE '\"%' AND rzg.\"Type\" = 'TXT') AND rzg.\"ZoneGroupId\" IS NOT NULL\n)\nUNION\nSELECT rz.\"ZoneId\" \nFROM \"Record\" rz\nWHERE (rz.\"Value\" LIKE '\"%' AND rz.\"Type\" = 'TXT') AND rz.\"ZoneId\" IS NOT NULL\n);");

            migrationBuilder.Sql(
                "UPDATE \"Record\" SET \"Value\" = SUBSTRING(\"Value\", 2, CHAR_LENGTH(\"Value\") - 2) WHERE \"Type\" = 'TXT' AND \"Value\" LIKE '\"%\"';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"Zones\" SET \"SerialNumber\" = \"SerialNumber\" + 1 WHERE \"Id\" IN (\nSELECT zgm.\"ZoneId\" FROM \"ZoneGroupMember\" zgm WHERE zgm.\"ZoneGroupId\" IN (\nSELECT rzg.\"ZoneGroupId\" \nFROM \"Record\" rzg\nWHERE (rzg.\"Type\" = 'TXT') AND rzg.\"ZoneGroupId\" IS NOT NULL\n)\nUNION\nSELECT rz.\"ZoneId\" \nFROM \"Record\" rz\nWHERE (rz.\"Type\" = 'TXT') AND rz.\"ZoneId\" IS NOT NULL\n);");

            migrationBuilder.Sql(
                "UPDATE \"Record\" SET \"Value\" = '\"' || \"Value\" || '\"' WHERE \"Type\" = 'TXT';");
        }
    }
}
