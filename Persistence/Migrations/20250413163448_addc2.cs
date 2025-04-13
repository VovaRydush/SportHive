using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addc2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

           

            

          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Team_Trainer_LoginTrainer",
                table: "Team");

            migrationBuilder.RenameColumn(
                name: "LoginTrainer",
                table: "Team",
                newName: "IdTraine");

            migrationBuilder.RenameIndex(
                name: "IX_Team_LoginTrainer",
                table: "Team",
                newName: "IX_Team_IdTraine");

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Trainer_IdTraine",
                table: "Team",
                column: "IdTraine",
                principalTable: "Trainer",
                principalColumn: "Login",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
