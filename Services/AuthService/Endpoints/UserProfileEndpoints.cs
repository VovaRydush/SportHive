using DB.SportHive.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportHive.Services.Interfaces;
using System.Data;

namespace AuthService.Endpoints
{
    public static class UserProfileEndpoints
    {
        public static void UserProfileEndpoint(this IEndpointRouteBuilder route)
        {
            route.MapGet("/get-user-photo/{login}", async (string login, [FromServices] IGetUserProfile userProfile) =>
            {
                return Results.Ok(await userProfile.GetUserPhoto(login));
            });

            route.MapGet("/get-statistic-info/{login}", async (string login, [FromServices] IGetUserProfile userProfile) =>
            {
                return Results.Ok(await userProfile.GetAllInfoUser(login));
            });

            route.MapGet("/get-search-athlete/{FullName}", async ([FromQuery] string FullName, [FromServices] IAthleteService userProfile) =>
            {
                return Results.Ok(await userProfile.SearchAthletesAsync(FullName));
            });

            route.MapGet("/profiles/member", async (AppDbContext db, string login, string role) =>
            {
                role = NormalizeRole(role);

                if (role != "Athlete" && role != "Trainer" && role != "Judge")
                    return Results.BadRequest("Invalid role");

                var person = await GetPerson(db, login, role);

                if (person is null)
                    return Results.NotFound("Profile not found");

                var organizations = await GetOrganizations(db, login, role);
                var teams = await GetTeams(db, login, role);
                var matches = await GetMatches(db, login, role);
                var judgedMatches = role == "Judge" ? await GetJudgedMatches(db, login) : new List<Dictionary<string, object?>>();
                var allMatches = role == "Judge" ? judgedMatches : matches;
                var upcoming = allMatches.Where(m => Convert.ToString(m.GetValueOrDefault("statusMatch")) != "2").ToList();
                var finished = allMatches.Where(m => Convert.ToString(m.GetValueOrDefault("statusMatch")) == "2").ToList();

                return Results.Ok(new
                {
                    login = person["login"],
                    role,
                    firstName = person["firstName"],
                    lastName = person["lastName"],
                    fullName = person["fullName"],
                    dataBirth = person["dataBirth"],
                    age = GetAge(person["dataBirth"]),
                    mail = person["mail"],
                    typeSport = person.GetValueOrDefault("typeSport"),
                    profilePhoto = person["profilePhoto"],
                    organizations,
                    teams,
                    matches,
                    judgedMatches,
                    upcomingMatches = upcoming,
                    finishedMatches = finished,
                    stats = BuildStats(allMatches, login, teams.Count, organizations.Count)
                });
            });
        }

        private static async Task<Dictionary<string, object?>?> GetPerson(AppDbContext db, string login, string role)
        {
            var typeSportSelect = role == "Athlete"
                ? """r."TypeSport" AS "typeSport","""
                : """NULL AS "typeSport",""";

            return (await QueryAsync(db, $"""
                SELECT
                    r."Login" AS "login",
                    r."FirsName" AS "firstName",
                    r."LastName" AS "lastName",
                    CONCAT(r."FirsName", ' ', r."LastName") AS "fullName",
                    r."DataBirth" AS "dataBirth",
                    u."mail" AS "mail",
                    {typeSportSelect}
                    up."ProfilePhoto" AS "profilePhoto"
                FROM "{role}" r
                JOIN "user" u ON u."login" = r."Login"
                LEFT JOIN "UserPhoto" up ON up."login" = r."Login"
                WHERE r."Login" = @login
                LIMIT 1
            """, ("@login", login))).FirstOrDefault();
        }

        private static async Task<List<Dictionary<string, object?>>> GetOrganizations(AppDbContext db, string login, string role)
        {
            if (role == "Trainer")
            {
                return await QueryAsync(db, """
                    SELECT
                        o."Login" AS "loginOrganization",
                        o."NameOrganization" AS "nameOrganization",
                        o."Country" AS "country",
                        o."TypeOrganozation" AS "typeOrganization",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationTrainer" ot
                    JOIN "Organization" o ON o."Login" = ot."LoginOrganization"
                    LEFT JOIN "UserPhoto" up ON up."login" = o."Login"
                    WHERE ot."LoginTraine" = @login
                    ORDER BY o."NameOrganization"
                """, ("@login", login));
            }

            if (role == "Judge")
            {
                return await QueryAsync(db, """
                    SELECT
                        o."Login" AS "loginOrganization",
                        o."NameOrganization" AS "nameOrganization",
                        o."Country" AS "country",
                        o."TypeOrganozation" AS "typeOrganization",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationJudge" oj
                    JOIN "Organization" o ON o."Login" = oj."LoginOrganization"
                    LEFT JOIN "UserPhoto" up ON up."login" = o."Login"
                    WHERE oj."LoginJudge" = @login
                    ORDER BY o."NameOrganization"
                """, ("@login", login));
            }

            return await QueryAsync(db, """
                SELECT
                    o."Login" AS "loginOrganization",
                    o."NameOrganization" AS "nameOrganization",
                    o."Country" AS "country",
                    o."TypeOrganozation" AS "typeOrganization",
                    up."ProfilePhoto" AS "profilePhoto"
                FROM "OrganizationAthlete" oa
                JOIN "Organization" o ON o."Login" = oa."LoginOrganization"
                LEFT JOIN "UserPhoto" up ON up."login" = o."Login"
                WHERE oa."LoginAthlete" = @login

                UNION

                SELECT
                    o."Login" AS "loginOrganization",
                    o."NameOrganization" AS "nameOrganization",
                    o."Country" AS "country",
                    o."TypeOrganozation" AS "typeOrganization",
                    up."ProfilePhoto" AS "profilePhoto"
                FROM "TeamAthlete" ta
                JOIN "OrganizationTeam" ot ON ot."NameComand" = ta."NameTeam"
                JOIN "Organization" o ON o."Login" = ot."LoginOrganization"
                LEFT JOIN "UserPhoto" up ON up."login" = o."Login"
                WHERE ta."IdAthlete" = @login

                ORDER BY "nameOrganization"
            """, ("@login", login));
        }

        private static async Task<List<Dictionary<string, object?>>> GetTeams(AppDbContext db, string login, string role)
        {
            if (role == "Trainer")
            {
                return await QueryAsync(db, """
                    SELECT
                        t."TeamName" AS "teamName",
                        t."TypeSport" AS "typeSport",
                        t."LoginTrainer" AS "loginTrainer",
                        t."TeamPhoto" AS "photoTeam",
                        COUNT(ta."IdAthlete") AS "athletesCount"
                    FROM "Team" t
                    LEFT JOIN "TeamAthlete" ta ON ta."NameTeam" = t."TeamName"
                    WHERE t."LoginTrainer" = @login
                    GROUP BY t."TeamName", t."TypeSport", t."LoginTrainer", t."TeamPhoto"
                    ORDER BY t."TeamName"
                """, ("@login", login));
            }

            if (role == "Athlete")
            {
                return await QueryAsync(db, """
                    SELECT
                        t."TeamName" AS "teamName",
                        t."TypeSport" AS "typeSport",
                        t."LoginTrainer" AS "loginTrainer",
                        t."TeamPhoto" AS "photoTeam",
                        COUNT(ta2."IdAthlete") AS "athletesCount"
                    FROM "TeamAthlete" ta
                    JOIN "Team" t ON t."TeamName" = ta."NameTeam"
                    LEFT JOIN "TeamAthlete" ta2 ON ta2."NameTeam" = t."TeamName"
                    WHERE ta."IdAthlete" = @login
                    GROUP BY t."TeamName", t."TypeSport", t."LoginTrainer", t."TeamPhoto"
                    ORDER BY t."TeamName"
                """, ("@login", login));
            }

            return new List<Dictionary<string, object?>>();
        }

        private static async Task<List<Dictionary<string, object?>>> GetMatches(AppDbContext db, string login, string role)
        {
            if (role == "Trainer")
            {
                return await QueryAsync(db, """
                    SELECT
                        tm."IdTeamMatch" AS "id",
                        tm."IdEvent" AS "idEvent",
                        e."NameEvent" AS "nameEvent",
                        'team' AS "type",
                        tm."NameFirstTeam" AS "firstSide",
                        tm."NameSecondTeam" AS "secondSide",
                        COALESCE(tm."AddInformation", '') AS "score",
                        tm."StatusMatch" AS "statusMatch",
                        tm."Tour" AS "tour",
                        tm."Group" AS "group",
                        tm."DataMatch" AS "dataMatch",
                        tm."TimeMatch" AS "timeMatch",
                        'trainer' AS "roleInMatch"
                    FROM "Team" t
                    JOIN "TeamMatch" tm ON tm."NameFirstTeam" = t."TeamName" OR tm."NameSecondTeam" = t."TeamName"
                    LEFT JOIN "Event" e ON e."IdEvent" = tm."IdEvent"
                    WHERE t."LoginTrainer" = @login
                    ORDER BY tm."DataMatch" DESC NULLS LAST, tm."Tour" DESC
                    LIMIT 50
                """, ("@login", login));
            }

            if (role == "Athlete")
            {
                var individual = await QueryAsync(db, """
                    SELECT
                        im."IdIndividualMatch" AS "id",
                        im."IdEvent" AS "idEvent",
                        e."NameEvent" AS "nameEvent",
                        'individual' AS "type",
                        im."loginFirstAthlete" AS "firstSide",
                        im."loginSecondAthlete" AS "secondSide",
                        COALESCE(im."AddInformation", '') AS "score",
                        im."StatusMatch" AS "statusMatch",
                        im."Tour" AS "tour",
                        im."Group" AS "group",
                        im."DataMatch" AS "dataMatch",
                        im."TimeMatch" AS "timeMatch",
                        'athlete' AS "roleInMatch"
                    FROM "IndividualMatch" im
                    LEFT JOIN "Event" e ON e."IdEvent" = im."IdEvent"
                    WHERE im."loginFirstAthlete" = @login OR im."loginSecondAthlete" = @login
                    ORDER BY im."DataMatch" DESC NULLS LAST, im."Tour" DESC
                    LIMIT 50
                """, ("@login", login));

                var team = await QueryAsync(db, """
                    SELECT
                        tm."IdTeamMatch" AS "id",
                        tm."IdEvent" AS "idEvent",
                        e."NameEvent" AS "nameEvent",
                        'team' AS "type",
                        tm."NameFirstTeam" AS "firstSide",
                        tm."NameSecondTeam" AS "secondSide",
                        COALESCE(tm."AddInformation", '') AS "score",
                        tm."StatusMatch" AS "statusMatch",
                        tm."Tour" AS "tour",
                        tm."Group" AS "group",
                        tm."DataMatch" AS "dataMatch",
                        tm."TimeMatch" AS "timeMatch",
                        'team athlete' AS "roleInMatch"
                    FROM "TeamAthlete" ta
                    JOIN "TeamMatch" tm ON tm."NameFirstTeam" = ta."NameTeam" OR tm."NameSecondTeam" = ta."NameTeam"
                    LEFT JOIN "Event" e ON e."IdEvent" = tm."IdEvent"
                    WHERE ta."IdAthlete" = @login
                    ORDER BY tm."DataMatch" DESC NULLS LAST, tm."Tour" DESC
                    LIMIT 50
                """, ("@login", login));

                individual.AddRange(team);
                return individual;
            }

            return new List<Dictionary<string, object?>>();
        }

        private static async Task<List<Dictionary<string, object?>>> GetJudgedMatches(AppDbContext db, string login)
        {
            var teamMatches = await QueryAsync(db, """
                SELECT
                    tm."IdTeamMatch" AS "id",
                    tm."IdEvent" AS "idEvent",
                    e."NameEvent" AS "nameEvent",
                    'team' AS "type",
                    tm."NameFirstTeam" AS "firstSide",
                    tm."NameSecondTeam" AS "secondSide",
                    COALESCE(tm."AddInformation", '') AS "score",
                    tm."StatusMatch" AS "statusMatch",
                    tm."Tour" AS "tour",
                    tm."Group" AS "group",
                    tm."DataMatch" AS "dataMatch",
                    tm."TimeMatch" AS "timeMatch",
                    'judge' AS "roleInMatch"
                FROM "TeamMatch" tm
                LEFT JOIN "Event" e ON e."IdEvent" = tm."IdEvent"
                WHERE tm."loginJudge" = @login
                ORDER BY tm."DataMatch" DESC NULLS LAST
                LIMIT 50
            """, ("@login", login));

            var individualMatches = await QueryAsync(db, """
                SELECT
                    im."IdIndividualMatch" AS "id",
                    im."IdEvent" AS "idEvent",
                    e."NameEvent" AS "nameEvent",
                    'individual' AS "type",
                    im."loginFirstAthlete" AS "firstSide",
                    im."loginSecondAthlete" AS "secondSide",
                    COALESCE(im."AddInformation", '') AS "score",
                    im."StatusMatch" AS "statusMatch",
                    im."Tour" AS "tour",
                    im."Group" AS "group",
                    im."DataMatch" AS "dataMatch",
                    im."TimeMatch" AS "timeMatch",
                    'judge' AS "roleInMatch"
                FROM "IndividualMatch" im
                LEFT JOIN "Event" e ON e."IdEvent" = im."IdEvent"
                WHERE im."loginJudge" = @login
                ORDER BY im."DataMatch" DESC NULLS LAST
                LIMIT 50
            """, ("@login", login));

            teamMatches.AddRange(individualMatches);
            return teamMatches;
        }

        private static object BuildStats(List<Dictionary<string, object?>> matches, string login, int teamsCount, int organizationsCount)
        {
            var total = matches.Count;
            var finished = 0;
            var wins = 0;
            var losses = 0;
            var draws = 0;
            var upcoming = 0;

            foreach (var match in matches)
            {
                var status = Convert.ToString(match.GetValueOrDefault("statusMatch")) ?? "0";

                if (status != "2")
                {
                    upcoming++;
                    continue;
                }

                finished++;

                var score = Convert.ToString(match.GetValueOrDefault("score")) ?? "";
                var parts = score.Split(':');

                if (parts.Length != 2 || !int.TryParse(parts[0], out var s1) || !int.TryParse(parts[1], out var s2))
                    continue;

                if (s1 == s2)
                {
                    draws++;
                    continue;
                }

                var firstSide = Convert.ToString(match.GetValueOrDefault("firstSide"));
                var isFirst = firstSide == login;

                if ((isFirst && s1 > s2) || (!isFirst && s2 > s1)) wins++;
                else losses++;
            }

            return new
            {
                totalMatches = total,
                upcomingMatches = upcoming,
                finishedMatches = finished,
                wins,
                losses,
                draws,
                teamsCount,
                organizationsCount
            };
        }

        private static int? GetAge(object? dataBirth)
        {
            if (dataBirth is null || !DateTime.TryParse(Convert.ToString(dataBirth), out var birth))
                return null;

            var today = DateTime.UtcNow.Date;
            var age = today.Year - birth.Year;

            if (birth.Date > today.AddYears(-age))
                age--;

            return age;
        }

        private static string NormalizeRole(string role)
        {
            if (role.Equals("Athlete", StringComparison.OrdinalIgnoreCase)) return "Athlete";
            if (role.Equals("Trainer", StringComparison.OrdinalIgnoreCase)) return "Trainer";
            if (role.Equals("Judge", StringComparison.OrdinalIgnoreCase)) return "Judge";
            return role;
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
