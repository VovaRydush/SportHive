using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changeIdOnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamAthlete_Team_IdTeam",
                table: "TeamAthlete");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamAthlete",
                table: "TeamAthlete");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Team",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Team");

            migrationBuilder.AlterColumn<int>(
                name: "IdTeam",
                table: "TeamAthlete",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "NameTeam",
                table: "TeamAthlete",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamAthlete",
                table: "TeamAthlete",
                columns: new[] { "NameTeam", "IdAthlete" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Team",
                table: "Team",
                column: "TeamName");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAthlete_Team_NameTeam",
                table: "TeamAthlete",
                column: "NameTeam",
                principalTable: "Team",
                principalColumn: "TeamName",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamAthlete_Team_NameTeam",
                table: "TeamAthlete");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamAthlete",
                table: "TeamAthlete");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Team",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "NameTeam",
                table: "TeamAthlete");

            migrationBuilder.AlterColumn<long>(
                name: "IdTeam",
                table: "TeamAthlete",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Team",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamAthlete",
                table: "TeamAthlete",
                columns: new[] { "IdTeam", "IdAthlete" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Team",
                table: "Team",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAthlete_Team_IdTeam",
                table: "TeamAthlete",
                column: "IdTeam",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
