using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addLoginToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_mail",
                table: "user");

            migrationBuilder.AddColumn<string>(
                name: "login",
                table: "user",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "login",
                table: "Athlete",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_user_login",
                table: "user",
                column: "login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_login",
                table: "user");

            migrationBuilder.DropColumn(
                name: "login",
                table: "user");

            migrationBuilder.DropColumn(
                name: "login",
                table: "Athlete");

            migrationBuilder.CreateIndex(
                name: "IX_user_mail",
                table: "user",
                column: "mail",
                unique: true);
        }
    }
}
