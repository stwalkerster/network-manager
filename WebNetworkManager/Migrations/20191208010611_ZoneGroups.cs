using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DnsWebApp.Migrations
{
    public partial class ZoneGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Class",
                table: "ZoneRecord");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ZoneRecord");

            migrationBuilder.DropColumn(
                name: "TimeToLive",
                table: "ZoneRecord");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ZoneRecord");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "ZoneRecord");

            migrationBuilder.AddColumn<long>(
                name: "RecordId",
                table: "ZoneRecord",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Record",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Class = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    TimeToLive = table.Column<long>(nullable: true),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Record", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZoneGroups",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZoneGroupMember",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ZoneId = table.Column<long>(nullable: false),
                    ZoneGroupId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneGroupMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZoneGroupMember_ZoneGroups_ZoneGroupId",
                        column: x => x.ZoneGroupId,
                        principalTable: "ZoneGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ZoneGroupMember_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZoneGroupRecord",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ZoneGroupId = table.Column<long>(nullable: false),
                    RecordId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneGroupRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZoneGroupRecord_Record_RecordId",
                        column: x => x.RecordId,
                        principalTable: "Record",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ZoneGroupRecord_ZoneGroups_ZoneGroupId",
                        column: x => x.ZoneGroupId,
                        principalTable: "ZoneGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ZoneRecord_RecordId",
                table: "ZoneRecord",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneGroupMember_ZoneGroupId",
                table: "ZoneGroupMember",
                column: "ZoneGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneGroupMember_ZoneId",
                table: "ZoneGroupMember",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneGroupRecord_RecordId",
                table: "ZoneGroupRecord",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneGroupRecord_ZoneGroupId",
                table: "ZoneGroupRecord",
                column: "ZoneGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ZoneRecord_Record_RecordId",
                table: "ZoneRecord",
                column: "RecordId",
                principalTable: "Record",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ZoneRecord_Record_RecordId",
                table: "ZoneRecord");

            migrationBuilder.DropTable(
                name: "ZoneGroupMember");

            migrationBuilder.DropTable(
                name: "ZoneGroupRecord");

            migrationBuilder.DropTable(
                name: "Record");

            migrationBuilder.DropTable(
                name: "ZoneGroups");

            migrationBuilder.DropIndex(
                name: "IX_ZoneRecord_RecordId",
                table: "ZoneRecord");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "ZoneRecord");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "ZoneRecord",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ZoneRecord",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TimeToLive",
                table: "ZoneRecord",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ZoneRecord",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "ZoneRecord",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
