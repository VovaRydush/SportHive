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
        teamName = Decode(teamName);
        var newName = string.IsNullOrWhiteSpace(request.NewTeamName) ? teamName : request.NewTeamName.Trim();

        await using var transaction = await _db.Database.BeginTransactionAsync(ct);

        var exists = await ScalarLongAsync("""
            SELECT COUNT(*) FROM "Team" WHERE "TeamName" = @team
        """, ct, ("@team", teamName));

        if (exists == 0)
        {
            return new ApiResultDto { Success = false, Message = "Команду не знайдено" };
        }

        if (!string.Equals(teamName, newName, StringComparison.Ordinal))
        {
            var targetExists = await ScalarLongAsync("""
                SELECT COUNT(*) FROM "Team" WHERE "TeamName" = @team
            """, ct, ("@team", newName));

            if (targetExists > 0)
            {
                return new ApiResultDto { Success = false, Message = "Команда з такою назвою вже існує" };
            }

            await ExecAsync("""
                INSERT INTO "Team" ("TeamName", "LoginTrainer", "TypeSport", "Trainerlogin", "TeamPhoto")
                SELECT
                    @newName,
                    COALESCE(NULLIF(@trainer, ''), "LoginTrainer"),
                    COALESCE(NULLIF(@sport, ''), "TypeSport"),
                    COALESCE(NULLIF(@trainer, ''), "Trainerlogin"),
                    COALESCE(@photo, "TeamPhoto")
                FROM "Team"
                WHERE "TeamName" = @oldName
            """, ct,
                ("@newName", newName),
                ("@oldName", teamName),
                ("@trainer", request.LoginTrainer ?? ""),
                ("@sport", request.TypeSport ?? ""),
                ("@photo", request.TeamPhoto));

            await ExecAsync("""
                UPDATE "TeamAthlete"
                SET "NameTeam" = @newName
                WHERE "NameTeam" = @oldName
            """, ct, ("@newName", newName), ("@oldName", teamName));

            await ExecAsync("""
                UPDATE "OrganizationTeam"
                SET "NameComand" = @newName
                WHERE "NameComand" = @oldName
            """, ct, ("@newName", newName), ("@oldName", teamName));

            await ExecIgnoreMissingAsync("""
                UPDATE "TeamMatch"
                SET "NameFirstTeam" = @newName
                WHERE "NameFirstTeam" = @oldName
            """, ct, ("@newName", newName), ("@oldName", teamName));

            await ExecIgnoreMissingAsync("""
                UPDATE "TeamMatch"
                SET "NameSecondTeam" = @newName
                WHERE "NameSecondTeam" = @oldName
            """, ct, ("@newName", newName), ("@oldName", teamName));

            await ExecAsync("""
                DELETE FROM "Team"
                WHERE "TeamName" = @oldName
            """, ct, ("@oldName", teamName));
        }
        else
        {
            await ExecAsync("""
                UPDATE "Team"
                SET "TypeSport" = COALESCE(NULLIF(@sport, ''), "TypeSport"),
                    "LoginTrainer" = COALESCE(NULLIF(@trainer, ''), "LoginTrainer"),
                    "Trainerlogin" = COALESCE(NULLIF(@trainer, ''), "Trainerlogin"),
                    "TeamPhoto" = COALESCE(@photo, "TeamPhoto")
                WHERE "TeamName" = @team
            """, ct,
                ("@team", teamName),
                ("@trainer", request.LoginTrainer ?? ""),
                ("@sport", request.TypeSport ?? ""),
                ("@photo", request.TeamPhoto));
        }

        await transaction.CommitAsync(ct);

        return new ApiResultDto
        {
            Success = true,
            ChangedRows = 1,
            Message = "Команду оновлено"
        };
    }

    public async Task<ApiResultDto> DeleteTeamAsync(string teamName, CancellationToken ct = default)
    {
        teamName = Decode(teamName);

        await using var transaction = await _db.Database.BeginTransactionAsync(ct);

        await ExecAsync("""DELETE FROM "TeamAthlete" WHERE "NameTeam" = @team""", ct, ("@team", teamName));
        await ExecAsync("""DELETE FROM "OrganizationTeam" WHERE "NameComand" = @team""", ct, ("@team", teamName));

        await ExecIgnoreMissingAsync("""
            DELETE FROM "TeamMatch"
            WHERE "NameFirstTeam" = @team OR "NameSecondTeam" = @team
        """, ct, ("@team", teamName));

        var changed = await ExecAsync("""DELETE FROM "Team" WHERE "TeamName" = @team""", ct, ("@team", teamName));

        await transaction.CommitAsync(ct);

        return new ApiResultDto
        {
            Success = changed > 0,
            ChangedRows = changed,
            Message = changed > 0 ? "Команду видалено" : "Команду не знайдено"
        };
    }

    public async Task<ApiResultDto> AddTeamAthleteAsync(string teamName, TeamAthleteRequest request, CancellationToken ct = default)
    {
        teamName = Decode(teamName);
        var athleteLogin = request.AthleteLogin?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(athleteLogin))
            return new ApiResultDto { Success = false, Message = "Login спортсмена пустий" };

        var teamExists = await ScalarLongAsync("""
            SELECT COUNT(*) FROM "Team" WHERE "TeamName" = @team
        """, ct, ("@team", teamName));

        if (teamExists == 0)
            return new ApiResultDto { Success = false, Message = "Команду не знайдено" };

        var athleteExists = await ScalarLongAsync("""
            SELECT COUNT(*) FROM "Athlete" WHERE "Login" = @login
        """, ct, ("@login", athleteLogin));

        if (athleteExists == 0)
            return new ApiResultDto { Success = false, Message = "Спортсмена з таким login не знайдено" };

        var idTeam = Math.Abs(teamName.GetHashCode());

        var changed = await ExecAsync("""
            INSERT INTO "TeamAthlete" ("IdTeam", "NameTeam", "IdAthlete", "AthleteStatus")
            VALUES (@idTeam, @team, @athlete, @status)
            ON CONFLICT ("NameTeam", "IdAthlete")
            DO UPDATE SET "AthleteStatus" = EXCLUDED."AthleteStatus"
        """, ct,
            ("@idTeam", idTeam),
            ("@team", teamName),
            ("@athlete", athleteLogin),
            ("@status", string.IsNullOrWhiteSpace(request.AthleteStatus) ? "Active" : request.AthleteStatus));

        return new ApiResultDto
        {
            Success = changed > 0,
            ChangedRows = changed,
            Message = "Склад команди оновлено"
        };
    }

    public async Task<ApiResultDto> RemoveTeamAthleteAsync(string teamName, string athleteLogin, CancellationToken ct = default)
    {
        teamName = Decode(teamName);
        athleteLogin = Decode(athleteLogin);

        var changed = await ExecAsync("""
            DELETE FROM "TeamAthlete"
            WHERE "NameTeam" = @team AND "IdAthlete" = @athlete
        """, ct, ("@team", teamName), ("@athlete", athleteLogin));

        return new ApiResultDto
        {
            Success = changed > 0,
            ChangedRows = changed,
            Message = changed > 0 ? "Спортсмена видалено з команди" : "Спортсмена в команді не знайдено"
        };
    }

    public async Task<ApiResultDto> AddOrganizationMemberAsync(string organizationLogin, OrganizationMemberRequest request, CancellationToken ct = default)
    {
        organizationLogin = Decode(organizationLogin);
        var role = NormalizeRole(request.Role);
        var login = request.Login?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(login))
            return new ApiResultDto { Success = false, Message = "Login пустий" };

        var orgExists = await ScalarLongAsync("""
            SELECT COUNT(*) FROM "Organization" WHERE "Login" = @org
        """, ct, ("@org", organizationLogin));

        if (orgExists == 0)
            return new ApiResultDto { Success = false, Message = "Організацію не знайдено" };

        var userTable = role switch
        {
            "Athlete" => "Athlete",
            "Trainer" => "Trainer",
            "Judge" => "Judge",
            _ => ""
        };

        if (string.IsNullOrWhiteSpace(userTable))
            return new ApiResultDto { Success = false, Message = "Невідома роль" };

        var userExists = await ScalarLongAsync($"""
            SELECT COUNT(*) FROM "{userTable}" WHERE "Login" = @login
        """, ct, ("@login", login));

        if (userExists == 0)
            return new ApiResultDto { Success = false, Message = "Користувача з такою роллю не знайдено" };

        var sql = role switch
        {
            "Athlete" => """INSERT INTO "OrganizationAthlete" ("LoginOrganization", "LoginAthlete") VALUES (@org, @login) ON CONFLICT ("LoginOrganization", "LoginAthlete") DO NOTHING""",
            "Trainer" => """INSERT INTO "OrganizationTrainer" ("LoginOrganization", "LoginTraine") VALUES (@org, @login) ON CONFLICT ("LoginOrganization", "LoginTraine") DO NOTHING""",
            "Judge" => """INSERT INTO "OrganizationJudge" ("LoginOrganization", "LoginJudge") VALUES (@org, @login) ON CONFLICT ("LoginJudge", "LoginOrganization") DO NOTHING""",
            _ => ""
        };

        var changed = await ExecAsync(sql, ct, ("@org", organizationLogin), ("@login", login));

        return new ApiResultDto
        {
            Success = true,
            ChangedRows = changed,
            Message = changed > 0 ? "Учасника додано в організацію" : "Учасник уже є в організації"
        };
    }

    public async Task<ApiResultDto> RemoveOrganizationMemberAsync(string organizationLogin, string role, string login, CancellationToken ct = default)
    {
        organizationLogin = Decode(organizationLogin);
        role = NormalizeRole(Decode(role));
        login = Decode(login);

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

        return new ApiResultDto
        {
            Success = changed > 0,
            ChangedRows = changed,
            Message = changed > 0 ? "Учасника видалено з організації" : "Зв'язок не знайдено"
        };
    }

    private static string NormalizeRole(string role)
    {
        role = (role ?? "").Trim();

        if (role.Equals("Athlete", StringComparison.OrdinalIgnoreCase) || role.Equals("спортсмен", StringComparison.OrdinalIgnoreCase)) return "Athlete";
        if (role.Equals("Trainer", StringComparison.OrdinalIgnoreCase) || role.Equals("тренер", StringComparison.OrdinalIgnoreCase)) return "Trainer";
        if (role.Equals("Judge", StringComparison.OrdinalIgnoreCase) || role.Equals("суддя", StringComparison.OrdinalIgnoreCase)) return "Judge";

        return role;
    }

    private static string Decode(string value)
    {
        return Uri.UnescapeDataString(value ?? "").Trim();
    }

    private async Task<long> ScalarLongAsync(string sql, CancellationToken ct, params (string Name, object? Value)[] parameters)
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

        var result = await command.ExecuteScalarAsync(ct);

        return Convert.ToInt64(result ?? 0);
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

    private async Task<int> ExecIgnoreMissingAsync(string sql, CancellationToken ct, params (string Name, object? Value)[] parameters)
    {
        try
        {
            return await ExecAsync(sql, ct, parameters);
        }
        catch
        {
            return 0;
        }
    }
}
