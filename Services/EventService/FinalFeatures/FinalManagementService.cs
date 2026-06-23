using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace SportHive.FinalFeatures;

public interface IFinalManagementService
{
    Task<ApiResultDto> UpdateTeamAsync(string teamName, TeamUpdateRequest request, CancellationToken ct = default);
    Task<ApiResultDto> DeleteTeamAsync(string teamName, CancellationToken ct = default);
    Task<ApiResultDto> AddTeamAthleteAsync(string teamName, TeamAthleteRequest request, CancellationToken ct = default);
    Task<ApiResultDto> RemoveTeamAthleteAsync(string teamName, string athleteLogin, CancellationToken ct = default);
    Task<ApiResultDto> AddOrganizationMemberAsync(string organizationLogin, OrganizationMemberRequest request, CancellationToken ct = default);
    Task<ApiResultDto> RemoveOrganizationMemberAsync(string organizationLogin, string role, string login, CancellationToken ct = default);
}

public sealed class FinalManagementService : IFinalManagementService
{
    private readonly AppDbContext _db;

    public FinalManagementService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResultDto> UpdateTeamAsync(string teamName, TeamUpdateRequest request, CancellationToken ct = default)
    {
        var newName = string.IsNullOrWhiteSpace(request.NewTeamName) ? teamName : request.NewTeamName.Trim();

        await using var transaction = await _db.Database.BeginTransactionAsync(ct);

        if (!string.Equals(teamName, newName, StringComparison.Ordinal))
        {
            await ExecAsync("""UPDATE "TeamAthlete" SET "NameTeam" = @newName WHERE "NameTeam" = @oldName""", ct, ("@newName", newName), ("@oldName", teamName));
            await ExecAsync("""UPDATE "OrganizationTeam" SET "NameComand" = @newName WHERE "NameComand" = @oldName""", ct, ("@newName", newName), ("@oldName", teamName));
            await ExecAsync("""UPDATE "TeamMatch" SET "NameFirstTeam" = @newName WHERE "NameFirstTeam" = @oldName""", ct, ("@newName", newName), ("@oldName", teamName));
            await ExecAsync("""UPDATE "TeamMatch" SET "NameSecondTeam" = @newName WHERE "NameSecondTeam" = @oldName""", ct, ("@newName", newName), ("@oldName", teamName));
        }

        var changed = await ExecAsync("""
            UPDATE "Team"
            SET "TeamName" = @newName,
                "TypeSport" = COALESCE(NULLIF(@sport, ''), "TypeSport"),
                "LoginTrainer" = COALESCE(NULLIF(@trainer, ''), "LoginTrainer"),
                "Trainerlogin" = COALESCE(NULLIF(@trainer, ''), "Trainerlogin"),
                "TeamPhoto" = COALESCE(@photo, "TeamPhoto")
            WHERE "TeamName" = @oldName OR "TeamName" = @newName
        """, ct,
            ("@newName", newName),
            ("@oldName", teamName),
            ("@sport", request.TypeSport ?? ""),
            ("@trainer", request.LoginTrainer ?? ""),
            ("@photo", request.TeamPhoto));

        await transaction.CommitAsync(ct);

        return new ApiResultDto { Success = changed > 0, ChangedRows = changed, Message = changed > 0 ? "Команду оновлено" : "Команду не знайдено" };
    }

    public async Task<ApiResultDto> DeleteTeamAsync(string teamName, CancellationToken ct = default)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(ct);

        await ExecAsync("""DELETE FROM "TeamAthlete" WHERE "NameTeam" = @team""", ct, ("@team", teamName));
        await ExecAsync("""DELETE FROM "OrganizationTeam" WHERE "NameComand" = @team""", ct, ("@team", teamName));
        await ExecAsync("""DELETE FROM "TeamMatch" WHERE "NameFirstTeam" = @team OR "NameSecondTeam" = @team""", ct, ("@team", teamName));

        var changed = await ExecAsync("""DELETE FROM "Team" WHERE "TeamName" = @team""", ct, ("@team", teamName));

        await transaction.CommitAsync(ct);

        return new ApiResultDto { Success = changed > 0, ChangedRows = changed, Message = changed > 0 ? "Команду видалено" : "Команду не знайдено" };
    }

    public async Task<ApiResultDto> AddTeamAthleteAsync(string teamName, TeamAthleteRequest request, CancellationToken ct = default)
    {
        var idTeam = Math.Abs(teamName.GetHashCode());

        var changed = await ExecAsync("""
            INSERT INTO "TeamAthlete" ("IdTeam", "NameTeam", "IdAthlete", "AthleteStatus")
            VALUES (@idTeam, @team, @athlete, @status)
            ON CONFLICT ("NameTeam", "IdAthlete")
            DO UPDATE SET "AthleteStatus" = EXCLUDED."AthleteStatus"
        """, ct,
            ("@idTeam", idTeam),
            ("@team", teamName),
            ("@athlete", request.AthleteLogin),
            ("@status", string.IsNullOrWhiteSpace(request.AthleteStatus) ? "Active" : request.AthleteStatus));

        return new ApiResultDto { Success = changed > 0, ChangedRows = changed, Message = "Спортсмена додано в команду" };
    }

    public async Task<ApiResultDto> RemoveTeamAthleteAsync(string teamName, string athleteLogin, CancellationToken ct = default)
    {
        var changed = await ExecAsync("""
            DELETE FROM "TeamAthlete"
            WHERE "NameTeam" = @team AND "IdAthlete" = @athlete
        """, ct, ("@team", teamName), ("@athlete", athleteLogin));

        return new ApiResultDto { Success = changed > 0, ChangedRows = changed, Message = changed > 0 ? "Спортсмена видалено з команди" : "Зв'язок не знайдено" };
    }

    public async Task<ApiResultDto> AddOrganizationMemberAsync(string organizationLogin, OrganizationMemberRequest request, CancellationToken ct = default)
    {
        var role = NormalizeRole(request.Role);
        var login = request.Login?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(login))
            return new ApiResultDto { Success = false, Message = "Login пустий" };

        var sql = role switch
        {
            "Athlete" => """INSERT INTO "OrganizationAthlete" ("LoginOrganization", "LoginAthlete") VALUES (@org, @login) ON CONFLICT ("LoginOrganization", "LoginAthlete") DO NOTHING""",
            "Trainer" => """INSERT INTO "OrganizationTrainer" ("LoginOrganization", "LoginTraine") VALUES (@org, @login) ON CONFLICT ("LoginOrganization", "LoginTraine") DO NOTHING""",
            "Judge" => """INSERT INTO "OrganizationJudge" ("LoginOrganization", "LoginJudge") VALUES (@org, @login) ON CONFLICT ("LoginJudge", "LoginOrganization") DO NOTHING""",
            _ => ""
        };

        if (string.IsNullOrWhiteSpace(sql))
            return new ApiResultDto { Success = false, Message = "Невідома роль" };

        var changed = await ExecAsync(sql, ct, ("@org", organizationLogin), ("@login", login));

        return new ApiResultDto { Success = true, ChangedRows = changed, Message = "Склад організації оновлено" };
    }

    public async Task<ApiResultDto> RemoveOrganizationMemberAsync(string organizationLogin, string role, string login, CancellationToken ct = default)
    {
        role = NormalizeRole(role);

        var sql = role switch
        {
            "Athlete" => """DELETE FROM "OrganizationAthlete" WHERE "LoginOrganization" = @org AND "LoginAthlete" = @login""",
            "Trainer" => """DELETE FROM "OrganizationTrainer" WHERE "LoginOrganization" = @org AND "LoginTraine" = @login""",
            "Judge" => """DELETE FROM "OrganizationJudge" WHERE "LoginOrganization" = @org AND "LoginJudge" = @login""",
            _ => ""
        };

        if (string.IsNullOrWhiteSpace(sql))
            return new ApiResultDto { Success = false, Message = "Невідома роль" };

        var changed = await ExecAsync(sql, ct, ("@org", organizationLogin), ("@login", login));

        return new ApiResultDto { Success = changed > 0, ChangedRows = changed, Message = changed > 0 ? "Учасника видалено" : "Зв'язок не знайдено" };
    }

    private static string NormalizeRole(string role)
    {
        role = (role ?? "").Trim();

        if (role.Equals("Athlete", StringComparison.OrdinalIgnoreCase) || role.Equals("спортсмен", StringComparison.OrdinalIgnoreCase)) return "Athlete";
        if (role.Equals("Trainer", StringComparison.OrdinalIgnoreCase) || role.Equals("тренер", StringComparison.OrdinalIgnoreCase)) return "Trainer";
        if (role.Equals("Judge", StringComparison.OrdinalIgnoreCase) || role.Equals("суддя", StringComparison.OrdinalIgnoreCase)) return "Judge";

        return role;
    }

    private async Task<int> ExecAsync(string sql, CancellationToken ct, params (string Name, object? Value)[] parameters)
    {
        var connection = _db.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        foreach (var (name, value) in parameters)
        {
            var p = command.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            command.Parameters.Add(p);
        }

        return await command.ExecuteNonQueryAsync(ct);
    }
}
