using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeFiled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtremeMatch_Location_LocationId",
                table: "ExtremeMatch");

            migrationBuilder.DropForeignKey(
                name: "FK_IndividualMatch_Location_LocationId",
                table: "IndividualMatch");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMatch_Location_LocationId",
                table: "TeamMatch");

            migrationBuilder.DropIndex(
                name: "IX_TeamMatch_LocationId",
                table: "TeamMatch");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Location",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_IndividualMatch_LocationId",
                table: "IndividualMatch");

            migrationBuilder.DropIndex(
                name: "IX_ExtremeMatch_LocationId",
                table: "ExtremeMatch");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "TeamMatch");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "IndividualMatch");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "ExtremeMatch");

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "TeamMatch",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "IndividualMatch",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "ExtremeMatch",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataEnd",
                table: "Event",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Location",
                table: "Location",
                column: "LocationName");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMatch_LocationName",
                table: "TeamMatch",
                column: "LocationName");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualMatch_LocationName",
                table: "IndividualMatch",
                column: "LocationName");

            migrationBuilder.CreateIndex(
                name: "IX_ExtremeMatch_LocationName",
                table: "ExtremeMatch",
                column: "LocationName");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtremeMatch_Location_LocationName",
                table: "ExtremeMatch",
                column: "LocationName",
                principalTable: "Location",
                principalColumn: "LocationName");

            migrationBuilder.AddForeignKey(
                name: "FK_IndividualMatch_Location_LocationName",
                table: "IndividualMatch",
                column: "LocationName",
                principalTable: "Location",
                principalColumn: "LocationName");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMatch_Location_LocationName",
                table: "TeamMatch",
                column: "LocationName",
                principalTable: "Location",
                principalColumn: "LocationName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtremeMatch_Location_LocationName",
                table: "ExtremeMatch");

            migrationBuilder.DropForeignKey(
                name: "FK_IndividualMatch_Location_LocationName",
                table: "IndividualMatch");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMatch_Location_LocationName",
                table: "TeamMatch");

            migrationBuilder.DropIndex(
                name: "IX_TeamMatch_LocationName",
                table: "TeamMatch");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Location",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_IndividualMatch_LocationName",
                table: "IndividualMatch");

            migrationBuilder.DropIndex(
                name: "IX_ExtremeMatch_LocationName",
                table: "ExtremeMatch");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "TeamMatch");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "IndividualMatch");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "ExtremeMatch");

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "TeamMatch",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Location",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "IndividualMatch",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "ExtremeMatch",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataEnd",
                table: "Event",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Location",
                table: "Location",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMatch_LocationId",
                table: "TeamMatch",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualMatch_LocationId",
                table: "IndividualMatch",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtremeMatch_LocationId",
                table: "ExtremeMatch",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtremeMatch_Location_LocationId",
                table: "ExtremeMatch",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IndividualMatch_Location_LocationId",
                table: "IndividualMatch",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMatch_Location_LocationId",
                table: "TeamMatch",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id");
        }
    }
}
