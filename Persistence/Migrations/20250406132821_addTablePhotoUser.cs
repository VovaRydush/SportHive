using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addTablePhotoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePhoto",
                table: "Trainer");

            migrationBuilder.DropColumn(
                name: "ProfilePhoto",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "ProfilePhoto",
                table: "Judge");

            migrationBuilder.DropColumn(
                name: "ProfilePhoto",
                table: "Athlete");

            migrationBuilder.CreateTable(
                name: "UserPhoto",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    ProfilePhoto = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhoto", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserPhoto_user_id",
                        column: x => x.id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPhoto");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhoto",
                table: "Trainer",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhoto",
                table: "Organization",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhoto",
                table: "Judge",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhoto",
                table: "Athlete",
                type: "text",
                nullable: true);
        }
    }
}
