using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OrgTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationTeam",
                columns: table => new
                {
                    LoginOrganization = table.Column<string>(type: "character varying(40)", nullable: false),
                    NameComand = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTeam", x => new { x.LoginOrganization, x.NameComand });
                    table.ForeignKey(
                        name: "FK_OrganizationTeam_Organization_LoginOrganization",
                        column: x => x.LoginOrganization,
                        principalTable: "Organization",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTeam_Team_NameComand",
                        column: x => x.NameComand,
                        principalTable: "Team",
                        principalColumn: "TeamName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTeam_NameComand",
                table: "OrganizationTeam",
                column: "NameComand");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationTeam");
        }
    }
}
