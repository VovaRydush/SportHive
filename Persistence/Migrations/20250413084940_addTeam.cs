using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamName = table.Column<string>(type: "text", nullable: false),
                    IdTraine = table.Column<long>(type: "bigint", nullable: false),
                    TypeSport = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TrainerId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Team_Trainer_IdTraine",
                        column: x => x.IdTraine,
                        principalTable: "Trainer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Team_Trainer_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainer",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "TeamAthlete",
                columns: table => new
                {
                    IdTeam = table.Column<long>(type: "bigint", nullable: false),
                    IdAthlete = table.Column<long>(type: "bigint", nullable: false),
                    AthleteStatus = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamAthlete", x => new { x.IdTeam, x.IdAthlete });
                    table.ForeignKey(
                        name: "FK_TeamAthlete_Athlete_IdAthlete",
                        column: x => x.IdAthlete,
                        principalTable: "Athlete",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamAthlete_Team_IdTeam",
                        column: x => x.IdTeam,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Team_IdTraine",
                table: "Team",
                column: "IdTraine",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Team_TrainerId",
                table: "Team",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAthlete_IdAthlete",
                table: "TeamAthlete",
                column: "IdAthlete");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamAthlete");

            migrationBuilder.DropTable(
                name: "Team");
        }
    }
}
