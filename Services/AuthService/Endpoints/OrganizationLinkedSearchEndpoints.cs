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
                return Results.Ok(await QueryAsync(db, """
                    SELECT
                        t."Login" AS "login",
                        CONCAT(t."FirsName", ' ', t."LastName") AS "fullName",
                        u."mail" AS "mail",
                        'Trainer' AS "role",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationTrainer" ot
                    JOIN "Trainer" t ON t."Login" = ot."LoginTraine"
                    JOIN "user" u ON u."login" = t."Login"
                    LEFT JOIN "UserPhoto" up ON up."login" = t."Login"
                    WHERE ot."LoginOrganization" = @org
                      AND (
                        @q = ''
                        OR LOWER(t."Login") LIKE LOWER(@like)
                        OR LOWER(t."FirsName") LIKE LOWER(@like)
                        OR LOWER(t."LastName") LIKE LOWER(@like)
                        OR LOWER(CONCAT(t."FirsName", ' ', t."LastName")) LIKE LOWER(@like)
                        OR LOWER(u."mail") LIKE LOWER(@like)
                      )
                    ORDER BY t."LastName", t."FirsName"
                    LIMIT 20
                """, ("@org", loginOrganization), ("@q", query ?? ""), ("@like", $"%{query ?? ""}%")));
            });

            group.MapGet("/athletes", async (AppDbContext db, string loginOrganization, string? query) =>
            {
                return Results.Ok(await QueryAsync(db, """
                    SELECT DISTINCT
                        a."Login" AS "login",
                        CONCAT(a."FirsName", ' ', a."LastName") AS "fullName",
                        a."TypeSport" AS "typeSport",
                        u."mail" AS "mail",
                        'Athlete' AS "role",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationAthlete" oa
                    JOIN "Athlete" a ON a."Login" = oa."LoginAthlete"
                    JOIN "user" u ON u."login" = a."Login"
                    LEFT JOIN "UserPhoto" up ON up."login" = a."Login"
                    WHERE oa."LoginOrganization" = @org
                      AND (
                        @q = ''
                        OR LOWER(a."Login") LIKE LOWER(@like)
                        OR LOWER(a."FirsName") LIKE LOWER(@like)
                        OR LOWER(a."LastName") LIKE LOWER(@like)
                        OR LOWER(CONCAT(a."FirsName", ' ', a."LastName")) LIKE LOWER(@like)
                        OR LOWER(a."TypeSport") LIKE LOWER(@like)
                        OR LOWER(u."mail") LIKE LOWER(@like)
                      )
                    ORDER BY a."TypeSport", a."LastName", a."FirsName"
                    LIMIT 20
                """, ("@org", loginOrganization), ("@q", query ?? ""), ("@like", $"%{query ?? ""}%")));
            });

            group.MapGet("/judges", async (AppDbContext db, string loginOrganization, string? query) =>
            {
                return Results.Ok(await QueryAsync(db, """
                    SELECT
                        j."Login" AS "login",
                        CONCAT(j."FirsName", ' ', j."LastName") AS "fullName",
                        u."mail" AS "mail",
                        'Judge' AS "role",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationJudge" oj
                    JOIN "Judge" j ON j."Login" = oj."LoginJudge"
                    JOIN "user" u ON u."login" = j."Login"
                    LEFT JOIN "UserPhoto" up ON up."login" = j."Login"
                    WHERE oj."LoginOrganization" = @org
                      AND (
                        @q = ''
                        OR LOWER(j."Login") LIKE LOWER(@like)
                        OR LOWER(j."FirsName") LIKE LOWER(@like)
                        OR LOWER(j."LastName") LIKE LOWER(@like)
                        OR LOWER(CONCAT(j."FirsName", ' ', j."LastName")) LIKE LOWER(@like)
                        OR LOWER(u."mail") LIKE LOWER(@like)
                      )
                    ORDER BY j."LastName", j."FirsName"
                    LIMIT 20
                """, ("@org", loginOrganization), ("@q", query ?? ""), ("@like", $"%{query ?? ""}%")));
            });

            group.MapGet("/teams", async (AppDbContext db, string loginOrganization, string? query) =>
            {
                return Results.Ok(await QueryAsync(db, """
                    SELECT
                        t."TeamName" AS "teamName",
                        t."TeamName" AS "name",
                        t."TypeSport" AS "typeSport",
                        t."LoginTrainer" AS "loginTrainer",
                        t."TeamPhoto" AS "teamPhoto"
                    FROM "OrganizationTeam" ot
                    JOIN "Team" t ON t."TeamName" = ot."NameComand"
                    WHERE ot."LoginOrganization" = @org
                      AND (
                        @q = ''
                        OR LOWER(t."TeamName") LIKE LOWER(@like)
                        OR LOWER(t."TypeSport") LIKE LOWER(@like)
                        OR LOWER(t."LoginTrainer") LIKE LOWER(@like)
                      )
                    ORDER BY t."TeamName"
                    LIMIT 20
                """, ("@org", loginOrganization), ("@q", query ?? ""), ("@like", $"%{query ?? ""}%")));
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
