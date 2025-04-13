using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addOrganizationJudge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationJudge",
                columns: table => new
                {
                    IdOrganization = table.Column<long>(type: "bigint", nullable: false),
                    IdJudge = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationJudge", x => new { x.IdJudge, x.IdOrganization });
                    table.ForeignKey(
                        name: "FK_OrganizationJudge_Judge_IdJudge",
                        column: x => x.IdJudge,
                        principalTable: "Judge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationJudge_Organization_IdOrganization",
                        column: x => x.IdOrganization,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationJudge_IdOrganization",
                table: "OrganizationJudge",
                column: "IdOrganization");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationJudge");
        }
    }
}
