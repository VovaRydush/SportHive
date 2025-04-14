using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OrgTeamMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationTrainer",
                columns: table => new
                {
                    LoginOrganization = table.Column<string>(type: "character varying(40)", nullable: false),
                    LoginTraine = table.Column<string>(type: "character varying(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationTrainer", x => new { x.LoginOrganization, x.LoginTraine });
                    table.ForeignKey(
                        name: "FK_OrganizationTrainer_Organization_LoginOrganization",
                        column: x => x.LoginOrganization,
                        principalTable: "Organization",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationTrainer_Trainer_LoginTraine",
                        column: x => x.LoginTraine,
                        principalTable: "Trainer",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationTrainer_LoginTraine",
                table: "OrganizationTrainer",
                column: "LoginTraine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationTrainer");
        }
    }
}
