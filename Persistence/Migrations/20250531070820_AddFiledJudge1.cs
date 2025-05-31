using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFiledJudge1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndividualMatch_Judge_loginJudge",
                table: "IndividualMatch");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMatch_Judge_loginJudge",
                table: "TeamMatch");

            migrationBuilder.AddForeignKey(
                name: "FK_IndividualMatch_Judge_loginJudge",
                table: "IndividualMatch",
                column: "loginJudge",
                principalTable: "Judge",
                principalColumn: "Login",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMatch_Judge_loginJudge",
                table: "TeamMatch",
                column: "loginJudge",
                principalTable: "Judge",
                principalColumn: "Login",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndividualMatch_Judge_loginJudge",
                table: "IndividualMatch");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMatch_Judge_loginJudge",
                table: "TeamMatch");

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
    }
}
