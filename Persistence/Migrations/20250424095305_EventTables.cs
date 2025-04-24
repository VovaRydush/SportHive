using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EventTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    IdEvent = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameEvent = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    systems = table.Column<string>(type: "text", nullable: false),
                    DataStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.IdEvent);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocationName = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtremeMatch",
                columns: table => new
                {
                    IdExtremeMatches = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEvent = table.Column<long>(type: "bigint", nullable: false),
                    DataMatch = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeMatch = table.Column<TimeSpan>(type: "interval", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    Tour = table.Column<int>(type: "integer", nullable: false),
                    AddInformation = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtremeMatch", x => x.IdExtremeMatches);
                    table.ForeignKey(
                        name: "FK_ExtremeMatch_Event_IdEvent",
                        column: x => x.IdEvent,
                        principalTable: "Event",
                        principalColumn: "IdEvent",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExtremeMatch_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IndividualMatch",
                columns: table => new
                {
                    IdIndividualMatch = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEvent = table.Column<long>(type: "bigint", nullable: false),
                    loginFirstAthlete = table.Column<string>(type: "character varying(40)", nullable: false),
                    loginSecondAthlete = table.Column<string>(type: "character varying(40)", nullable: false),
                    DataMatch = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeMatch = table.Column<TimeSpan>(type: "interval", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    Tour = table.Column<int>(type: "integer", nullable: false),
                    AddInformation = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualMatch", x => x.IdIndividualMatch);
                    table.ForeignKey(
                        name: "FK_IndividualMatch_Athlete_loginFirstAthlete",
                        column: x => x.loginFirstAthlete,
                        principalTable: "Athlete",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndividualMatch_Athlete_loginSecondAthlete",
                        column: x => x.loginSecondAthlete,
                        principalTable: "Athlete",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndividualMatch_Event_IdEvent",
                        column: x => x.IdEvent,
                        principalTable: "Event",
                        principalColumn: "IdEvent");
                    table.ForeignKey(
                        name: "FK_IndividualMatch_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TeamMatch",
                columns: table => new
                {
                    IdTeamMatch = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEvent = table.Column<long>(type: "bigint", nullable: false),
                    NameFirstTeam = table.Column<string>(type: "text", nullable: false),
                    NameSecondTeam = table.Column<string>(type: "text", nullable: false),
                    DataMatch = table.Column<DateTime>(type: "date", nullable: false),
                    TimeMatch = table.Column<TimeSpan>(type: "time", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    Tour = table.Column<int>(type: "integer", nullable: false),
                    AddInformation = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMatch", x => x.IdTeamMatch);
                    table.ForeignKey(
                        name: "FK_TeamMatch_Event_IdEvent",
                        column: x => x.IdEvent,
                        principalTable: "Event",
                        principalColumn: "IdEvent");
                    table.ForeignKey(
                        name: "FK_TeamMatch_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeamMatch_Team_NameFirstTeam",
                        column: x => x.NameFirstTeam,
                        principalTable: "Team",
                        principalColumn: "TeamName");
                    table.ForeignKey(
                        name: "FK_TeamMatch_Team_NameSecondTeam",
                        column: x => x.NameSecondTeam,
                        principalTable: "Team",
                        principalColumn: "TeamName");
                });

            migrationBuilder.CreateTable(
                name: "EMatchesAthlete",
                columns: table => new
                {
                    loginAthlete = table.Column<string>(type: "character varying(40)", nullable: false),
                    IdExtremeMatches = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMatchesAthlete", x => new { x.loginAthlete, x.IdExtremeMatches });
                    table.ForeignKey(
                        name: "FK_EMatchesAthlete_Athlete_loginAthlete",
                        column: x => x.loginAthlete,
                        principalTable: "Athlete",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EMatchesAthlete_ExtremeMatch_IdExtremeMatches",
                        column: x => x.IdExtremeMatches,
                        principalTable: "ExtremeMatch",
                        principalColumn: "IdExtremeMatches",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EMatchesAthlete_IdExtremeMatches",
                table: "EMatchesAthlete",
                column: "IdExtremeMatches");

            migrationBuilder.CreateIndex(
                name: "IX_Event_IdEvent",
                table: "Event",
                column: "IdEvent",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExtremeMatch_IdEvent",
                table: "ExtremeMatch",
                column: "IdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_ExtremeMatch_LocationId",
                table: "ExtremeMatch",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualMatch_IdEvent",
                table: "IndividualMatch",
                column: "IdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualMatch_LocationId",
                table: "IndividualMatch",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualMatch_loginFirstAthlete",
                table: "IndividualMatch",
                column: "loginFirstAthlete");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualMatch_loginSecondAthlete",
                table: "IndividualMatch",
                column: "loginSecondAthlete");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMatch_IdEvent",
                table: "TeamMatch",
                column: "IdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMatch_LocationId",
                table: "TeamMatch",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMatch_NameFirstTeam",
                table: "TeamMatch",
                column: "NameFirstTeam");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMatch_NameSecondTeam",
                table: "TeamMatch",
                column: "NameSecondTeam");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EMatchesAthlete");

            migrationBuilder.DropTable(
                name: "IndividualMatch");

            migrationBuilder.DropTable(
                name: "TeamMatch");

            migrationBuilder.DropTable(
                name: "ExtremeMatch");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
