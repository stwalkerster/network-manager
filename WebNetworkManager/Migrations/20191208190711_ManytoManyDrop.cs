using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DnsWebApp.Migrations
{
    public partial class ManytoManyDrop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ZoneGroupRecord");

            migrationBuilder.DropTable(
                name: "ZoneRecord");

            migrationBuilder.AddColumn<long>(
                name: "ZoneGroupId",
                table: "Record",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ZoneId",
                table: "Record",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Record_ZoneGroupId",
                table: "Record",
                column: "ZoneGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Record_ZoneId",
                table: "Record",
                column: "ZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Record_ZoneGroups_ZoneGroupId",
                table: "Record",
                column: "ZoneGroupId",
                principalTable: "ZoneGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Record_Zones_ZoneId",
                table: "Record",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Record_ZoneGroups_ZoneGroupId",
                table: "Record");

            migrationBuilder.DropForeignKey(
                name: "FK_Record_Zones_ZoneId",
                table: "Record");

            migrationBuilder.DropIndex(
                name: "IX_Record_ZoneGroupId",
                table: "Record");

            migrationBuilder.DropIndex(
                name: "IX_Record_ZoneId",
                table: "Record");

            migrationBuilder.DropColumn(
                name: "ZoneGroupId",
                table: "Record");

            migrationBuilder.DropColumn(
                name: "ZoneId",
                table: "Record");

            migrationBuilder.CreateTable(
                name: "ZoneGroupRecord",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecordId = table.Column<long>(type: "bigint", nullable: false),
                    ZoneGroupId = table.Column<long>(type: "bigint", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ZoneRecord",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecordId = table.Column<long>(type: "bigint", nullable: false),
                    ZoneId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZoneRecord_Record_RecordId",
                        column: x => x.RecordId,
                        principalTable: "Record",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ZoneRecord_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ZoneGroupRecord_RecordId",
                table: "ZoneGroupRecord",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneGroupRecord_ZoneGroupId",
                table: "ZoneGroupRecord",
                column: "ZoneGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneRecord_RecordId",
                table: "ZoneRecord",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneRecord_ZoneId",
                table: "ZoneRecord",
                column: "ZoneId");
        }
    }
}
