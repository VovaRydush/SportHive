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

            group.MapGet("/judges", async (AppDbContext db, string loginOrganization, string? query) =>
            {
                return Results.Ok(await QueryAsync(db, """
                    SELECT
                        j."Login" AS "login",
                        CONCAT(j."FirsName", ' ', j."LastName") AS "fullName",
                        'Judge' AS "role",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationJudge" oj
                    JOIN "Judge" j ON j."Login" = oj."LoginJudge"
                    LEFT JOIN "UserPhoto" up ON up."login" = j."Login"
                    WHERE oj."LoginOrganization" = @org
                      AND (
                        @q = ''
                        OR LOWER(j."Login") LIKE LOWER(@like)
                        OR LOWER(j."FirsName") LIKE LOWER(@like)
                        OR LOWER(j."LastName") LIKE LOWER(@like)
                        OR LOWER(CONCAT(j."FirsName", ' ', j."LastName")) LIKE LOWER(@like)
                      )
                    ORDER BY j."LastName", j."FirsName"
                    LIMIT 20
                """, loginOrganization, query));
            });

            group.MapGet("/trainers", async (AppDbContext db, string loginOrganization, string? query) =>
            {
                return Results.Ok(await QueryAsync(db, """
                    SELECT
                        t."Login" AS "login",
                        CONCAT(t."FirsName", ' ', t."LastName") AS "fullName",
                        'Trainer' AS "role",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationTrainer" ot
                    JOIN "Trainer" t ON t."Login" = ot."LoginTraine"
                    LEFT JOIN "UserPhoto" up ON up."login" = t."Login"
                    WHERE ot."LoginOrganization" = @org
                      AND (
                        @q = ''
                        OR LOWER(t."Login") LIKE LOWER(@like)
                        OR LOWER(t."FirsName") LIKE LOWER(@like)
                        OR LOWER(t."LastName") LIKE LOWER(@like)
                        OR LOWER(CONCAT(t."FirsName", ' ', t."LastName")) LIKE LOWER(@like)
                      )
                    ORDER BY t."LastName", t."FirsName"
                    LIMIT 20
                """, loginOrganization, query));
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
                """, loginOrganization, query));
            });

            group.MapGet("/athletes", async (AppDbContext db, string loginOrganization, string? query) =>
            {
                return Results.Ok(await QueryAsync(db, """
                    SELECT DISTINCT
                        a."Login" AS "login",
                        CONCAT(a."FirsName", ' ', a."LastName") AS "fullName",
                        a."TypeSport" AS "typeSport",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationTeam" ot
                    JOIN "TeamAthlete" ta ON ta."NameTeam" = ot."NameComand"
                    JOIN "Athlete" a ON a."Login" = ta."IdAthlete"
                    LEFT JOIN "UserPhoto" up ON up."login" = a."Login"
                    WHERE ot."LoginOrganization" = @org
                      AND (
                        @q = ''
                        OR LOWER(a."Login") LIKE LOWER(@like)
                        OR LOWER(a."FirsName") LIKE LOWER(@like)
                        OR LOWER(a."LastName") LIKE LOWER(@like)
                        OR LOWER(CONCAT(a."FirsName", ' ', a."LastName")) LIKE LOWER(@like)
                      )
                    ORDER BY "fullName"
                    LIMIT 20
                """, loginOrganization, query));
            });
        }

        private static async Task<List<Dictionary<string, object?>>> QueryAsync(
            AppDbContext db,
            string sql,
            string loginOrganization,
            string? query)
        {
            var result = new List<Dictionary<string, object?>>();
            var connection = db.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            Add(command, "@org", loginOrganization);
            Add(command, "@q", query ?? "");
            Add(command, "@like", $"%{query ?? ""}%");

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

        private static void Add(IDbCommand command, string name, object? value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }
}
