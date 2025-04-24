using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LocationStartEnd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LatitudeEnd",
                table: "Location",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationNameEnd",
                table: "Location",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LongitudeEnd",
                table: "Location",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatitudeEnd",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "LocationNameEnd",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "LongitudeEnd",
                table: "Location");
        }
    }
}
