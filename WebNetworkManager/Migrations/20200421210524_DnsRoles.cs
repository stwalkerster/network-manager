using Microsoft.EntityFrameworkCore.Migrations;

namespace DnsWebApp.Migrations
{
    public partial class DnsRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"AspNetUserRoles\" WHERE 1=1;");
            migrationBuilder.Sql("DELETE FROM \"AspNetRoles\" WHERE 1=1;");
            
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "ced2b534-7eb7-42cf-a7bb-f9e644a8a2c9", "f8059916-d868-4531-9298-93eb5179577e", "Administrator", "ADMINISTRATOR" },
                    { "6dcde805-ed4f-4cd4-96c6-bacd13e898cd", "0538e13f-ab27-4004-a319-4ead42314926", "DNS Manager", "DNS MANAGER" },
                    { "78f7cc77-fb36-429e-baae-2c2a85330b74", "ea0fcdb6-f463-4ca9-9dd9-0cdf88f92d82", "DNS User", "DNS USER" },
                    { "50301cad-be60-44e0-baf4-df63f7f0c9c5", "759bb383-a20d-4e30-87a9-8d0641123542", "Static Data", "STATIC DATA" }
                });

            migrationBuilder.Sql(
                "INSERT INTO \"AspNetUserRoles\" SELECT \"Id\", 'ced2b534-7eb7-42cf-a7bb-f9e644a8a2c9' FROM \"AspNetUsers\"");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"AspNetUserRoles\" WHERE 1=1;");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "50301cad-be60-44e0-baf4-df63f7f0c9c5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6dcde805-ed4f-4cd4-96c6-bacd13e898cd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "78f7cc77-fb36-429e-baae-2c2a85330b74");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ced2b534-7eb7-42cf-a7bb-f9e644a8a2c9");
        }
    }
}
