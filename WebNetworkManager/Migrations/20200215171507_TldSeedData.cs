using Microsoft.EntityFrameworkCore.Migrations;

namespace DnsWebApp.Migrations
{
    public partial class TldSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TopLevelDomains",
                columns: new[] { "Id", "Domain", "Registry", "RegistryUrl", "WhoisServer" },
                values: new object[,]
                {
                    { -1L, "com", "Verisign", "https://www.verisign.com/en_US/domain-names/com-domain-names/index.xhtml", "whois.verisign-grs.com" },
                    { -2L, "net", "Verisign", "https://www.verisign.com/en_US/domain-names/net-domain-names/index.xhtml", "whois.verisign-grs.com" },
                    { -3L, "org", "Public Interest Registry", "https://thenew.org/org-people/", "whois.pir.org" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TopLevelDomains",
                keyColumn: "Id",
                keyValue: -3L);

            migrationBuilder.DeleteData(
                table: "TopLevelDomains",
                keyColumn: "Id",
                keyValue: -2L);

            migrationBuilder.DeleteData(
                table: "TopLevelDomains",
                keyColumn: "Id",
                keyValue: -1L);
        }
    }
}
