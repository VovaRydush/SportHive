using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Command.Endpoints
{
    public static class TeamViewEndpoints
    {
        public static void TeamViewEndpoint(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/team");

            group.MapGet("/by-organization/{loginOrganization}", async (AppDbContext db, string loginOrganization) =>
            {
                var teams = await QueryAsync(db, """
                    SELECT
                        t."TeamName" AS "nameTeam",
                        t."TypeSport" AS "typeSport",
                        t."LoginTrainer" AS "loginTrainer",
                        t."TeamPhoto" AS "photoTeam",
                        COUNT(ta."IdAthlete") AS "athletesCount"
                    FROM "OrganizationTeam" ot
                    JOIN "Team" t ON t."TeamName" = ot."NameComand"
                    LEFT JOIN "TeamAthlete" ta ON ta."NameTeam" = t."TeamName"
                    WHERE ot."LoginOrganization" = @org
                    GROUP BY t."TeamName", t."TypeSport", t."LoginTrainer", t."TeamPhoto"
                    ORDER BY t."TeamName"
                """, ("@org", loginOrganization));

                return Results.Ok(teams);
            });

            group.MapGet("/by-trainer/{loginTrainer}", async (AppDbContext db, string loginTrainer) =>
            {
                var teams = await QueryAsync(db, """
                    SELECT
                        t."TeamName" AS "nameTeam",
                        t."TypeSport" AS "typeSport",
                        t."LoginTrainer" AS "loginTrainer",
                        t."TeamPhoto" AS "photoTeam",
                        COUNT(ta."IdAthlete") AS "athletesCount"
                    FROM "Team" t
                    LEFT JOIN "TeamAthlete" ta ON ta."NameTeam" = t."TeamName"
                    WHERE t."LoginTrainer" = @trainer
                    GROUP BY t."TeamName", t."TypeSport", t."LoginTrainer", t."TeamPhoto"
                    ORDER BY t."TeamName"
                """, ("@trainer", loginTrainer));

                return Results.Ok(teams);
            });

            group.MapGet("/{nameTeam}", async (AppDbContext db, string nameTeam) =>
            {
                var team = (await QueryAsync(db, """
                    SELECT
                        t."TeamName" AS "nameTeam",
                        t."TypeSport" AS "typeSport",
                        t."LoginTrainer" AS "trainerLogin",
                        tr."FirsName" AS "trainerFirstName",
                        tr."LastName" AS "trainerLastName",
                        up."ProfilePhoto" AS "trainerPhoto",
                        t."TeamPhoto" AS "photoTeam"
                    FROM "Team" t
                    LEFT JOIN "Trainer" tr ON tr."Login" = t."LoginTrainer"
                    LEFT JOIN "UserPhoto" up ON up."login" = tr."Login"
                    WHERE t."TeamName" = @team
                    LIMIT 1
                """, ("@team", nameTeam))).FirstOrDefault();

                if (team is null)
                    return Results.NotFound("Team not found");

                var athletes = await QueryAsync(db, """
                    SELECT
                        a."Login" AS "login",
                        CONCAT(a."FirsName", ' ', a."LastName") AS "fullName",
                        a."FirsName" AS "firsName",
                        a."LastName" AS "lastName",
                        a."TypeSport" AS "typeSport",
                        ta."AthleteStatus" AS "athleteStatus",
                        up."ProfilePhoto" AS "photo"
                    FROM "TeamAthlete" ta
                    JOIN "Athlete" a ON a."Login" = ta."IdAthlete"
                    LEFT JOIN "UserPhoto" up ON up."login" = a."Login"
                    WHERE ta."NameTeam" = @team
                    ORDER BY a."LastName", a."FirsName"
                """, ("@team", nameTeam));

                var organizations = await QueryAsync(db, """
                    SELECT
                        o."Login" AS "loginOrganization",
                        o."NameOrganization" AS "nameOrganization",
                        o."Country" AS "country",
                        o."TypeOrganozation" AS "typeOrganization",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationTeam" ot
                    JOIN "Organization" o ON o."Login" = ot."LoginOrganization"
                    LEFT JOIN "UserPhoto" up ON up."login" = o."Login"
                    WHERE ot."NameComand" = @team
                    ORDER BY o."NameOrganization"
                """, ("@team", nameTeam));

                var matches = await QueryAsync(db, """
                    SELECT
                        tm."IdTeamMatch" AS "id",
                        tm."IdEvent" AS "idEvent",
                        e."NameEvent" AS "nameEvent",
                        tm."NameFirstTeam" AS "firstTeam",
                        tm."NameSecondTeam" AS "secondTeam",
                        tm."StatusMatch" AS "statusMatch",
                        tm."Tour" AS "tour",
                        tm."Group" AS "group",
                        tm."DataMatch" AS "dataMatch",
                        tm."TimeMatch" AS "timeMatch",
                        COALESCE(tm."AddInformation", '') AS "score"
                    FROM "TeamMatch" tm
                    LEFT JOIN "Event" e ON e."IdEvent" = tm."IdEvent"
                    WHERE tm."NameFirstTeam" = @team OR tm."NameSecondTeam" = @team
                    ORDER BY tm."DataMatch" DESC NULLS LAST, tm."Tour" DESC
                    LIMIT 50
                """, ("@team", nameTeam));

                return Results.Ok(new
                {
                    nameTeam = team["nameTeam"],
                    typeSport = team["typeSport"],
                    trainerLogin = team["trainerLogin"],
                    trainerFirstName = team["trainerFirstName"],
                    trainerLastName = team["trainerLastName"],
                    trainerPhoto = team["trainerPhoto"],
                    photoTeam = team["photoTeam"],
                    athletes,
                    organizations,
                    athletesCount = athletes.Count,
                    matches,
                    stats = BuildStats(matches, nameTeam)
                });
            });
        }

        private static object BuildStats(List<Dictionary<string, object?>> matches, string teamName)
        {
            var total = matches.Count;
            var finished = 0;
            var wins = 0;
            var losses = 0;
            var draws = 0;

            foreach (var match in matches)
            {
                var status = Convert.ToString(match.GetValueOrDefault("statusMatch")) ?? "0";

                if (status != "2")
                    continue;

                finished++;

                var score = Convert.ToString(match.GetValueOrDefault("score")) ?? "";
                var parts = score.Split(':');

                if (parts.Length != 2 || !int.TryParse(parts[0], out var s1) || !int.TryParse(parts[1], out var s2))
                    continue;

                var firstTeam = Convert.ToString(match.GetValueOrDefault("firstTeam"));
                var isFirst = firstTeam == teamName;

                if (s1 == s2) draws++;
                else if ((isFirst && s1 > s2) || (!isFirst && s2 > s1)) wins++;
                else losses++;
            }

            return new
            {
                totalMatches = total,
                finishedMatches = finished,
                wins,
                losses,
                draws
            };
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
