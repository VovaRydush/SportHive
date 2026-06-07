using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AuthService.Endpoints
{
    public static class OrganizationLinkedSearchEndpoints
    {
        public static void OrganizationLinkedSearchEndpoint(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/organization-linked");

            group.MapGet("/trainers", async (AppDbContext db, string loginOrganization, string? query) =>
            {
                query ??= "";

                return Results.Ok(await QueryAsync(db, """
                    SELECT DISTINCT
                        t."Login" AS "login",
                        CONCAT(t."FirsName", ' ', t."LastName") AS "fullName",
                        u."mail" AS "mail",
                        'Trainer' AS "role",
                        'organization' AS "source",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationTrainer" ot
                    JOIN "Trainer" t ON t."Login" = ot."LoginTraine"
                    JOIN "user" u ON u."login" = t."Login"
                    LEFT JOIN "UserPhoto" up ON up."login" = t."Login"
                    WHERE ot."LoginOrganization" = @org
                      AND (
                        @q = ''
                        OR t."Login" ILIKE @like
                        OR t."FirsName" ILIKE @like
                        OR t."LastName" ILIKE @like
                        OR CONCAT(t."FirsName", ' ', t."LastName") ILIKE @like
                        OR u."mail" ILIKE @like
                      )
                    ORDER BY "fullName"
                    LIMIT 30
                """, ("@org", loginOrganization), ("@q", query), ("@like", $"%{query}%")));
            });

            group.MapGet("/athletes", async (AppDbContext db, string loginOrganization, string? query) =>
            {
                query ??= "";

                // This intentionally returns:
                // 1. athletes directly accepted into organization;
                // 2. athletes from organization teams;
                // 3. accepted athlete invitations;
                // 4. fallback global athletes, marked as "global", so CreateTeam search works even if OrganizationAthlete was not filled yet.
                return Results.Ok(await QueryAsync(db, """
                    WITH org_athletes AS (
                        SELECT oa."LoginAthlete" AS login, 'organization' AS source
                        FROM "OrganizationAthlete" oa
                        WHERE oa."LoginOrganization" = @org

                        UNION

                        SELECT ta."IdAthlete" AS login, 'team' AS source
                        FROM "OrganizationTeam" ot
                        JOIN "TeamAthlete" ta ON ta."NameTeam" = ot."NameComand"
                        WHERE ot."LoginOrganization" = @org

                        UNION

                        SELECT oi."TargetLogin" AS login, 'accepted invitation' AS source
                        FROM "OrganizationInvitation" oi
                        WHERE oi."LoginOrganization" = @org
                          AND oi."TargetRole" = 'Athlete'
                          AND oi."Status" = 'Accepted'
                    ),
                    result AS (
                        SELECT
                            a."Login" AS "login",
                            CONCAT(a."FirsName", ' ', a."LastName") AS "fullName",
                            a."TypeSport" AS "typeSport",
                            u."mail" AS "mail",
                            'Athlete' AS "role",
                            oa.source AS "source",
                            up."ProfilePhoto" AS "profilePhoto"
                        FROM org_athletes oa
                        JOIN "Athlete" a ON a."Login" = oa.login
                        JOIN "user" u ON u."login" = a."Login"
                        LEFT JOIN "UserPhoto" up ON up."login" = a."Login"

                        UNION

                        SELECT
                            a."Login" AS "login",
                            CONCAT(a."FirsName", ' ', a."LastName") AS "fullName",
                            a."TypeSport" AS "typeSport",
                            u."mail" AS "mail",
                            'Athlete' AS "role",
                            'global' AS "source",
                            up."ProfilePhoto" AS "profilePhoto"
                        FROM "Athlete" a
                        JOIN "user" u ON u."login" = a."Login"
                        LEFT JOIN "UserPhoto" up ON up."login" = a."Login"
                    )
                    SELECT DISTINCT ON ("login")
                        "login",
                        "fullName",
                        "typeSport",
                        "mail",
                        "role",
                        "source",
                        "profilePhoto"
                    FROM result
                    WHERE (
                        @q = ''
                        OR "login" ILIKE @like
                        OR "fullName" ILIKE @like
                        OR "typeSport" ILIKE @like
                        OR "mail" ILIKE @like
                    )
                    ORDER BY "login",
                        CASE "source"
                            WHEN 'organization' THEN 1
                            WHEN 'team' THEN 2
                            WHEN 'accepted invitation' THEN 3
                            ELSE 4
                        END
                    LIMIT 30
                """, ("@org", loginOrganization), ("@q", query), ("@like", $"%{query}%")));
            });

            group.MapGet("/judges", async (AppDbContext db, string loginOrganization, string? query) =>
            {
                query ??= "";

                return Results.Ok(await QueryAsync(db, """
                    SELECT DISTINCT
                        j."Login" AS "login",
                        CONCAT(j."FirsName", ' ', j."LastName") AS "fullName",
                        u."mail" AS "mail",
                        'Judge' AS "role",
                        'organization' AS "source",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationJudge" oj
                    JOIN "Judge" j ON j."Login" = oj."LoginJudge"
                    JOIN "user" u ON u."login" = j."Login"
                    LEFT JOIN "UserPhoto" up ON up."login" = j."Login"
                    WHERE oj."LoginOrganization" = @org
                      AND (
                        @q = ''
                        OR j."Login" ILIKE @like
                        OR j."FirsName" ILIKE @like
                        OR j."LastName" ILIKE @like
                        OR CONCAT(j."FirsName", ' ', j."LastName") ILIKE @like
                        OR u."mail" ILIKE @like
                      )
                    ORDER BY "fullName"
                    LIMIT 30
                """, ("@org", loginOrganization), ("@q", query), ("@like", $"%{query}%")));
            });

            group.MapGet("/teams", async (AppDbContext db, string loginOrganization, string? query) =>
            {
                query ??= "";

                return Results.Ok(await QueryAsync(db, """
                    SELECT DISTINCT
                        t."TeamName" AS "teamName",
                        t."TeamName" AS "name",
                        t."TypeSport" AS "typeSport",
                        t."LoginTrainer" AS "loginTrainer",
                        t."TeamPhoto" AS "teamPhoto",
                        'organization' AS "source"
                    FROM "OrganizationTeam" ot
                    JOIN "Team" t ON t."TeamName" = ot."NameComand"
                    WHERE ot."LoginOrganization" = @org
                      AND (
                        @q = ''
                        OR t."TeamName" ILIKE @like
                        OR t."TypeSport" ILIKE @like
                        OR t."LoginTrainer" ILIKE @like
                      )
                    ORDER BY t."TeamName"
                    LIMIT 30
                """, ("@org", loginOrganization), ("@q", query), ("@like", $"%{query}%")));
            });
        }

        private static async Task<List<Dictionary<string, object?>>> QueryAsync(
            AppDbContext db,
            string sql,
            params (string Name, object? Value)[] parameters)
        {
            var result = new List<Dictionary<string, object?>>();
            var connection = db.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            foreach (var (name, value) in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = name;
                parameter.Value = value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();

                for (var i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);

                result.Add(row);
            }

            return result;
        }
    }
}
