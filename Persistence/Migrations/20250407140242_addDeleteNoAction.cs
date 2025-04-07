using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addDeleteNoAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Athlete_user_Id",
                table: "Athlete");

            migrationBuilder.DropForeignKey(
                name: "FK_Judge_user_Id",
                table: "Judge");

            migrationBuilder.DropForeignKey(
                name: "FK_Organization_user_Id",
                table: "Organization");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainer_user_id",
                table: "Trainer");

            migrationBuilder.AddForeignKey(
                name: "FK_Athlete_user_Id",
                table: "Athlete",
                column: "Id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Judge_user_Id",
                table: "Judge",
                column: "Id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_user_Id",
                table: "Organization",
                column: "Id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainer_user_id",
                table: "Trainer",
                column: "id",
                principalTable: "user",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Athlete_user_Id",
                table: "Athlete");

            migrationBuilder.DropForeignKey(
                name: "FK_Judge_user_Id",
                table: "Judge");

            migrationBuilder.DropForeignKey(
                name: "FK_Organization_user_Id",
                table: "Organization");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainer_user_id",
                table: "Trainer");

            migrationBuilder.AddForeignKey(
                name: "FK_Athlete_user_Id",
                table: "Athlete",
                column: "Id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Judge_user_Id",
                table: "Judge",
                column: "Id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_user_Id",
                table: "Organization",
                column: "Id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainer_user_id",
                table: "Trainer",
                column: "id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
