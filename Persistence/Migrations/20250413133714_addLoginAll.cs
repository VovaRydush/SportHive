using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addLoginAll : Migration
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
                name: "FK_OrganizationJudge_Judge_IdJudge",
                table: "OrganizationJudge");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationJudge_Organization_IdOrganization",
                table: "OrganizationJudge");

            migrationBuilder.DropForeignKey(
                name: "FK_Team_Trainer_IdTraine",
                table: "Team");

            migrationBuilder.DropForeignKey(
                name: "FK_Team_Trainer_TrainerId",
                table: "Team");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAthlete_Athlete_IdAthlete",
                table: "TeamAthlete");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainer_user_id",
                table: "Trainer");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPhoto_user_id",
                table: "UserPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPhoto",
                table: "UserPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trainer",
                table: "Trainer");

            migrationBuilder.DropIndex(
                name: "IX_Team_TrainerId",
                table: "Team");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationJudge",
                table: "OrganizationJudge");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationJudge_IdOrganization",
                table: "OrganizationJudge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organization",
                table: "Organization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Judge",
                table: "Judge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Athlete",
                table: "Athlete");

            migrationBuilder.DropColumn(
                name: "id",
                table: "UserPhoto");

            migrationBuilder.DropColumn(
                name: "id",
                table: "Trainer");

            migrationBuilder.DropColumn(
                name: "TrainerId",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "IdJudge",
                table: "OrganizationJudge");

            migrationBuilder.DropColumn(
                name: "IdOrganization",
                table: "OrganizationJudge");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Judge");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Athlete");

            migrationBuilder.RenameColumn(
                name: "login",
                table: "Athlete",
                newName: "Login");

            migrationBuilder.AddColumn<string>(
                name: "login",
                table: "UserPhoto",
                type: "character varying(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Trainer",
                type: "character varying(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "IdAthlete",
                table: "TeamAthlete",
                type: "character varying(40)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "IdTraine",
                table: "Team",
                type: "character varying(40)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "Trainerlogin",
                table: "Team",
                type: "character varying(40)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginJudge",
                table: "OrganizationJudge",
                type: "character varying(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LoginOrganization",
                table: "OrganizationJudge",
                type: "character varying(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Organization",
                type: "character varying(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Judge",
                type: "character varying(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPhoto",
                table: "UserPhoto",
                column: "login");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user",
                table: "user",
                column: "login");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trainer",
                table: "Trainer",
                column: "Login");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationJudge",
                table: "OrganizationJudge",
                columns: new[] { "LoginJudge", "LoginOrganization" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organization",
                table: "Organization",
                column: "Login");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Judge",
                table: "Judge",
                column: "Login");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Athlete",
                table: "Athlete",
                column: "Login");

            migrationBuilder.CreateIndex(
                name: "IX_Team_Trainerlogin",
                table: "Team",
                column: "Trainerlogin");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationJudge_LoginOrganization",
                table: "OrganizationJudge",
                column: "LoginOrganization");

            migrationBuilder.AddForeignKey(
                name: "FK_Athlete_user_Login",
                table: "Athlete",
                column: "Login",
                principalTable: "user",
                principalColumn: "login");

            migrationBuilder.AddForeignKey(
                name: "FK_Judge_user_Login",
                table: "Judge",
                column: "Login",
                principalTable: "user",
                principalColumn: "login");

            migrationBuilder.AddForeignKey(
                name: "FK_Organization_user_Login",
                table: "Organization",
                column: "Login",
                principalTable: "user",
                principalColumn: "login");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationJudge_Judge_LoginJudge",
                table: "OrganizationJudge",
                column: "LoginJudge",
                principalTable: "Judge",
                principalColumn: "Login",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationJudge_Organization_LoginOrganization",
                table: "OrganizationJudge",
                column: "LoginOrganization",
                principalTable: "Organization",
                principalColumn: "Login",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Trainer_IdTraine",
                table: "Team",
                column: "IdTraine",
                principalTable: "Trainer",
                principalColumn: "Login",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Trainer_Trainerlogin",
                table: "Team",
                column: "Trainerlogin",
                principalTable: "Trainer",
                principalColumn: "Login");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAthlete_Athlete_IdAthlete",
                table: "TeamAthlete",
                column: "IdAthlete",
                principalTable: "Athlete",
                principalColumn: "Login",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainer_user_Login",
                table: "Trainer",
                column: "Login",
                principalTable: "user",
                principalColumn: "login");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPhoto_user_login",
                table: "UserPhoto",
                column: "login",
                principalTable: "user",
                principalColumn: "login",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Athlete_user_Login",
                table: "Athlete");

            migrationBuilder.DropForeignKey(
                name: "FK_Judge_user_Login",
                table: "Judge");

            migrationBuilder.DropForeignKey(
                name: "FK_Organization_user_Login",
                table: "Organization");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationJudge_Judge_LoginJudge",
                table: "OrganizationJudge");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationJudge_Organization_LoginOrganization",
                table: "OrganizationJudge");

            migrationBuilder.DropForeignKey(
                name: "FK_Team_Trainer_IdTraine",
                table: "Team");

            migrationBuilder.DropForeignKey(
                name: "FK_Team_Trainer_Trainerlogin",
                table: "Team");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamAthlete_Athlete_IdAthlete",
                table: "TeamAthlete");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainer_user_Login",
                table: "Trainer");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPhoto_user_login",
                table: "UserPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPhoto",
                table: "UserPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trainer",
                table: "Trainer");

            migrationBuilder.DropIndex(
                name: "IX_Team_Trainerlogin",
                table: "Team");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationJudge",
                table: "OrganizationJudge");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationJudge_LoginOrganization",
                table: "OrganizationJudge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organization",
                table: "Organization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Judge",
                table: "Judge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Athlete",
                table: "Athlete");

            migrationBuilder.DropColumn(
                name: "login",
                table: "UserPhoto");

            migrationBuilder.DropColumn(
                name: "Login",
                table: "Trainer");

            migrationBuilder.DropColumn(
                name: "Trainerlogin",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "LoginJudge",
                table: "OrganizationJudge");

            migrationBuilder.DropColumn(
                name: "LoginOrganization",
                table: "OrganizationJudge");

            migrationBuilder.DropColumn(
                name: "Login",
                table: "Organization");

            migrationBuilder.DropColumn(
                name: "Login",
                table: "Judge");

            migrationBuilder.RenameColumn(
                name: "Login",
                table: "Athlete",
                newName: "login");

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "UserPhoto",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "Trainer",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "IdAthlete",
                table: "TeamAthlete",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)");

            migrationBuilder.AlterColumn<long>(
                name: "IdTraine",
                table: "Team",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)");

            migrationBuilder.AddColumn<long>(
                name: "TrainerId",
                table: "Team",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IdJudge",
                table: "OrganizationJudge",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "IdOrganization",
                table: "OrganizationJudge",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Organization",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Judge",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Athlete",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPhoto",
                table: "UserPhoto",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user",
                table: "user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trainer",
                table: "Trainer",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationJudge",
                table: "OrganizationJudge",
                columns: new[] { "IdJudge", "IdOrganization" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organization",
                table: "Organization",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Judge",
                table: "Judge",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Athlete",
                table: "Athlete",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Team_TrainerId",
                table: "Team",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationJudge_IdOrganization",
                table: "OrganizationJudge",
                column: "IdOrganization");

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
                name: "FK_OrganizationJudge_Judge_IdJudge",
                table: "OrganizationJudge",
                column: "IdJudge",
                principalTable: "Judge",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationJudge_Organization_IdOrganization",
                table: "OrganizationJudge",
                column: "IdOrganization",
                principalTable: "Organization",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Trainer_IdTraine",
                table: "Team",
                column: "IdTraine",
                principalTable: "Trainer",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Team_Trainer_TrainerId",
                table: "Team",
                column: "TrainerId",
                principalTable: "Trainer",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamAthlete_Athlete_IdAthlete",
                table: "TeamAthlete",
                column: "IdAthlete",
                principalTable: "Athlete",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainer_user_id",
                table: "Trainer",
                column: "id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPhoto_user_id",
                table: "UserPhoto",
                column: "id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
