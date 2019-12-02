using Microsoft.EntityFrameworkCore.Migrations;

namespace LdapDnsWebApp.Migrations
{
    public partial class RegistrarUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PrimaryNameServer",
                table: "Zones",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Administrator",
                table: "Zones",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PrimaryNameServer",
                table: "Zones",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Administrator",
                table: "Zones",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
