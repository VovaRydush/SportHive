using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class users1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "user_details");

            migrationBuilder.AlterColumn<string>(
                name: "refreshToken",
                table: "user",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "HashPassword",
                table: "user",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Athlete",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FirsName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    LastName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    ProfilePhoto = table.Column<string>(type: "text", nullable: false),
                    DataBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Athlete", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Athlete_user_Id",
                        column: x => x.Id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Judge",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    FirsName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    LastName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    ProfilePhoto = table.Column<string>(type: "text", nullable: false),
                    DataBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Judge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Judge_user_Id",
                        column: x => x.Id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    NameOrganization = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TypeOrganozation = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    ProfilePhoto = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateFoundation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organization_user_Id",
                        column: x => x.Id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trainer",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    FirsName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    LastName = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    ProfilePhoto = table.Column<string>(type: "text", nullable: false),
                    DataBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainer", x => x.id);
                    table.ForeignKey(
                        name: "FK_Trainer_user_id",
                        column: x => x.id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organization_NameOrganization",
                table: "Organization",
                column: "NameOrganization");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Athlete");

            migrationBuilder.DropTable(
                name: "Judge");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "Trainer");

            migrationBuilder.AlterColumn<string>(
                name: "refreshToken",
                table: "user",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "HashPassword",
                table: "user",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_details",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DataBirhsday = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    ProfilePhoto = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_details", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_user_details_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_user_id",
                table: "RefreshToken",
                column: "user_id");
        }
    }
}
