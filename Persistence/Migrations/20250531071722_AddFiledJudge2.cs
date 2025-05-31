using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFiledJudge2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtremeMatch_Judge_loginJudge",
                table: "ExtremeMatch");

            migrationBuilder.AlterColumn<string>(
                name: "loginJudge",
                table: "TeamMatch",
                type: "character varying(40)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)");

            migrationBuilder.AlterColumn<string>(
                name: "loginJudge",
                table: "IndividualMatch",
                type: "character varying(40)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)");

            migrationBuilder.AlterColumn<string>(
                name: "loginJudge",
                table: "ExtremeMatch",
                type: "character varying(40)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtremeMatch_Judge_loginJudge",
                table: "ExtremeMatch",
                column: "loginJudge",
                principalTable: "Judge",
                principalColumn: "Login",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtremeMatch_Judge_loginJudge",
                table: "ExtremeMatch");

            migrationBuilder.AlterColumn<string>(
                name: "loginJudge",
                table: "TeamMatch",
                type: "character varying(40)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "loginJudge",
                table: "IndividualMatch",
                type: "character varying(40)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "loginJudge",
                table: "ExtremeMatch",
                type: "character varying(40)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExtremeMatch_Judge_loginJudge",
                table: "ExtremeMatch",
                column: "loginJudge",
                principalTable: "Judge",
                principalColumn: "Login",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
