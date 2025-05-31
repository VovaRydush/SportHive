using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFiledJudge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "loginJudge",
                table: "TeamMatch",
                type: "character varying(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "loginJudge",
                table: "IndividualMatch",
                type: "character varying(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "loginJudge",
                table: "ExtremeMatch",
                type: "character varying(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMatch_loginJudge",
                table: "TeamMatch",
                column: "loginJudge");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualMatch_loginJudge",
                table: "IndividualMatch",
                column: "loginJudge");

            migrationBuilder.CreateIndex(
                name: "IX_ExtremeMatch_loginJudge",
                table: "ExtremeMatch",
                column: "loginJudge");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtremeMatch_Judge_loginJudge",
                table: "ExtremeMatch",
                column: "loginJudge",
                principalTable: "Judge",
                principalColumn: "Login",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IndividualMatch_Judge_loginJudge",
                table: "IndividualMatch",
                column: "loginJudge",
                principalTable: "Judge",
                principalColumn: "Login",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMatch_Judge_loginJudge",
                table: "TeamMatch",
                column: "loginJudge",
                principalTable: "Judge",
                principalColumn: "Login",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtremeMatch_Judge_loginJudge",
                table: "ExtremeMatch");

            migrationBuilder.DropForeignKey(
                name: "FK_IndividualMatch_Judge_loginJudge",
                table: "IndividualMatch");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMatch_Judge_loginJudge",
                table: "TeamMatch");

            migrationBuilder.DropIndex(
                name: "IX_TeamMatch_loginJudge",
                table: "TeamMatch");

            migrationBuilder.DropIndex(
                name: "IX_IndividualMatch_loginJudge",
                table: "IndividualMatch");

            migrationBuilder.DropIndex(
                name: "IX_ExtremeMatch_loginJudge",
                table: "ExtremeMatch");

            migrationBuilder.DropColumn(
                name: "loginJudge",
                table: "TeamMatch");

            migrationBuilder.DropColumn(
                name: "loginJudge",
                table: "IndividualMatch");

            migrationBuilder.DropColumn(
                name: "loginJudge",
                table: "ExtremeMatch");
        }
    }
}
