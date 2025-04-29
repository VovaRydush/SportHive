using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEMatchTeamTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EMatchesTeam",
                columns: table => new
                {
                    NameTeam = table.Column<string>(type: "text", nullable: false),
                    IdExtremeMatches = table.Column<long>(type: "bigint", nullable: false),
                    TeamName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMatchesTeam", x => new { x.NameTeam, x.IdExtremeMatches });
                    table.ForeignKey(
                        name: "FK_EMatchesTeam_ExtremeMatch_IdExtremeMatches",
                        column: x => x.IdExtremeMatches,
                        principalTable: "ExtremeMatch",
                        principalColumn: "IdExtremeMatches",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EMatchesTeam_Team_NameTeam",
                        column: x => x.NameTeam,
                        principalTable: "Team",
                        principalColumn: "TeamName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EMatchesTeam_Team_TeamName",
                        column: x => x.TeamName,
                        principalTable: "Team",
                        principalColumn: "TeamName");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EMatchesTeam_IdExtremeMatches",
                table: "EMatchesTeam",
                column: "IdExtremeMatches");

            migrationBuilder.CreateIndex(
                name: "IX_EMatchesTeam_TeamName",
                table: "EMatchesTeam",
                column: "TeamName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EMatchesTeam");
        }
    }
}
