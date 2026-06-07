using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;
using System.Text.Json;
using Confluent.Kafka;

namespace AuthService.Endpoints
{
    public static class OrganizationInvitationEndpoints
    {
        public static void OrganizationInvitationEndpoint(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/organization");

            group.MapGet("/profile-data", async (AppDbContext db, string loginOrganization) =>
            {
                var org = (await QueryAsync(db, """
                    SELECT
                        o."Login" AS "login",
                        o."NameOrganization" AS "nameOrganization",
                        o."TypeOrganozation" AS "typeOrganization",
                        o."Description" AS "description",
                        o."Country" AS "country",
                        o."DateFoundation" AS "dateFoundation",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "Organization" o
                    LEFT JOIN "UserPhoto" up ON up."login" = o."Login"
                    WHERE o."Login" = @org
                    LIMIT 1
                """, ("@org", loginOrganization))).FirstOrDefault();

                if (org is null)
                    return Results.NotFound("Organization not found");

                var judges = await QueryAsync(db, """
                    SELECT
                        j."Login" AS "login",
                        CONCAT(j."FirsName", ' ', j."LastName") AS "fullName",
                        u."mail" AS "mail",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationJudge" oj
                    JOIN "Judge" j ON j."Login" = oj."LoginJudge"
                    JOIN "user" u ON u."login" = j."Login"
                    LEFT JOIN "UserPhoto" up ON up."login" = j."Login"
                    WHERE oj."LoginOrganization" = @org
                    ORDER BY j."LastName", j."FirsName"
                """, ("@org", loginOrganization));

                var trainers = await QueryAsync(db, """
                    SELECT
                        t."Login" AS "login",
                        CONCAT(t."FirsName", ' ', t."LastName") AS "fullName",
                        u."mail" AS "mail",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationTrainer" ot
                    JOIN "Trainer" t ON t."Login" = ot."LoginTraine"
                    JOIN "user" u ON u."login" = t."Login"
                    LEFT JOIN "UserPhoto" up ON up."login" = t."Login"
                    WHERE ot."LoginOrganization" = @org
                    ORDER BY t."LastName", t."FirsName"
                """, ("@org", loginOrganization));

                var athletes = await QueryAsync(db, """
                    SELECT
                        a."Login" AS "login",
                        CONCAT(a."FirsName", ' ', a."LastName") AS "fullName",
                        a."TypeSport" AS "typeSport",
                        u."mail" AS "mail",
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "OrganizationAthlete" oa
                    JOIN "Athlete" a ON a."Login" = oa."LoginAthlete"
                    JOIN "user" u ON u."login" = a."Login"
                    LEFT JOIN "UserPhoto" up ON up."login" = a."Login"
                    WHERE oa."LoginOrganization" = @org
                    ORDER BY a."TypeSport", a."LastName", a."FirsName"
                """, ("@org", loginOrganization));

                var teams = await QueryAsync(db, """
                    SELECT
                        t."TeamName" AS "teamName",
                        t."TypeSport" AS "typeSport",
                        t."LoginTrainer" AS "loginTrainer",
                        t."TeamPhoto" AS "teamPhoto",
                        COUNT(ta."IdAthlete") AS "athletesCount"
                    FROM "OrganizationTeam" ot
                    JOIN "Team" t ON t."TeamName" = ot."NameComand"
                    LEFT JOIN "TeamAthlete" ta ON ta."NameTeam" = t."TeamName"
                    WHERE ot."LoginOrganization" = @org
                    GROUP BY t."TeamName", t."TypeSport", t."LoginTrainer", t."TeamPhoto"
                    ORDER BY t."TeamName"
                """, ("@org", loginOrganization));

                var invitations = await QueryAsync(db, """
                    SELECT
                        "Id" AS "id",
                        "LoginOrganization" AS "loginOrganization",
                        "TargetLogin" AS "targetLogin",
                        "TargetEmail" AS "targetEmail",
                        "TargetRole" AS "targetRole",
                        "Status" AS "status",
                        "CreatedAt" AS "createdAt",
                        "RespondedAt" AS "respondedAt"
                    FROM "OrganizationInvitation"
                    WHERE "LoginOrganization" = @org
                    ORDER BY "CreatedAt" DESC
                    LIMIT 50
                """, ("@org", loginOrganization));

                var recentEvents = await QueryAsync(db, """
                    WITH org_teams AS (
                        SELECT "NameComand" AS name FROM "OrganizationTeam" WHERE "LoginOrganization" = @org
                    ),
                    org_athletes AS (
                        SELECT "LoginAthlete" AS login FROM "OrganizationAthlete" WHERE "LoginOrganization" = @org
                    ),
                    org_events AS (
                        SELECT DISTINCT e."IdEvent"
                        FROM "Event" e
                        LEFT JOIN "TeamMatch" tm ON tm."IdEvent" = e."IdEvent"
                        LEFT JOIN "IndividualMatch" im ON im."IdEvent" = e."IdEvent"
                        WHERE tm."NameFirstTeam" IN (SELECT name FROM org_teams)
                           OR tm."NameSecondTeam" IN (SELECT name FROM org_teams)
                           OR im."loginFirstAthlete" IN (SELECT login FROM org_athletes)
                           OR im."loginSecondAthlete" IN (SELECT login FROM org_athletes)
                    )
                    SELECT
                        e."IdEvent" AS "idEvent",
                        e."NameEvent" AS "nameEvent",
                        e."TypeSport" AS "typeSport",
                        e.systems AS "systems",
                        e."DataStart" AS "dataStart",
                        e."DataEnd" AS "dataEnd",
                        0 AS "finishedMatches",
                        0 AS "totalMatches"
                    FROM "Event" e
                    JOIN org_events oe ON oe."IdEvent" = e."IdEvent"
                    ORDER BY e."DataStart" DESC
                    LIMIT 10
                """, ("@org", loginOrganization));

                var athletesBySport = athletes
                    .GroupBy(a => Convert.ToString(a.GetValueOrDefault("typeSport")) ?? "Інше")
                    .ToDictionary(g => g.Key, g => g.ToList());

                return Results.Ok(new
                {
                    organization = org,
                    judges,
                    trainers,
                    athletesBySport,
                    teams,
                    invitations,
                    recentEvents
                });
            });

            group.MapGet("/search-users", async (AppDbContext db, string loginOrganization, string role, string? query) =>
            {
                role = NormalizeRole(role);
                query ??= "";

                if (role != "Athlete" && role != "Judge" && role != "Trainer")
                    return Results.BadRequest("Invalid role");

                var existingSql = role switch
                {
                    "Judge" => """SELECT "LoginJudge" FROM "OrganizationJudge" WHERE "LoginOrganization" = @org""",
                    "Trainer" => """SELECT "LoginTraine" FROM "OrganizationTrainer" WHERE "LoginOrganization" = @org""",
                    _ => """SELECT "LoginAthlete" FROM "OrganizationAthlete" WHERE "LoginOrganization" = @org"""
                };

                var typeSportSelect = role == "Athlete"
                    ? """r."TypeSport" AS "typeSport","""
                    : """NULL AS "typeSport",""";

                var sql = $"""
                    SELECT
                        r."Login" AS "login",
                        CONCAT(r."FirsName", ' ', r."LastName") AS "fullName",
                        u."mail" AS "mail",
                        u."Role" AS "role",
                        {typeSportSelect}
                        up."ProfilePhoto" AS "profilePhoto"
                    FROM "{role}" r
                    JOIN "user" u ON u."login" = r."Login"
                    LEFT JOIN "UserPhoto" up ON up."login" = r."Login"
                    WHERE r."Login" NOT IN ({existingSql})
                      AND r."Login" NOT IN (
                          SELECT "TargetLogin"
                          FROM "OrganizationInvitation"
                          WHERE "LoginOrganization" = @org
                            AND "TargetRole" = @role
                            AND "Status" = 'Pending'
                      )
                      AND (
                        @q = ''
                        OR LOWER(r."Login") LIKE LOWER(@like)
                        OR LOWER(r."FirsName") LIKE LOWER(@like)
                        OR LOWER(r."LastName") LIKE LOWER(@like)
                        OR LOWER(CONCAT(r."FirsName", ' ', r."LastName")) LIKE LOWER(@like)
                        OR LOWER(u."mail") LIKE LOWER(@like)
                      )
                    ORDER BY r."LastName", r."FirsName"
                    LIMIT 20
                """;

                var users = await QueryAsync(db, sql,
                    ("@org", loginOrganization),
                    ("@role", role),
                    ("@q", query),
                    ("@like", $"%{query}%"));

                return Results.Ok(users);
            });

            group.MapPost("/invitations", async (
                AppDbContext db,
                IConfiguration config,
                OrganizationInviteRequest request) =>
            {
                var role = NormalizeRole(request.TargetRole);

                if (role != "Athlete" && role != "Judge" && role != "Trainer")
                    return Results.BadRequest("Invalid role");

                var target = (await QueryAsync(db, $"""
                    SELECT
                        r."Login" AS "login",
                        CONCAT(r."FirsName", ' ', r."LastName") AS "fullName",
                        u."mail" AS "mail",
                        u."Role" AS "role"
                    FROM "{role}" r
                    JOIN "user" u ON u."login" = r."Login"
                    WHERE r."Login" = @login
                    LIMIT 1
                """, ("@login", request.TargetLogin))).FirstOrDefault();

                if (target is null)
                    return Results.NotFound("Target user not found");

                var targetEmail = Convert.ToString(target["mail"]) ?? "";
                var targetName = Convert.ToString(target["fullName"]) ?? request.TargetLogin;
                var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();

                await ExecuteAsync(db, """
                    INSERT INTO "OrganizationInvitation"
                    (
                        "LoginOrganization",
                        "TargetLogin",
                        "TargetEmail",
                        "TargetRole",
                        "Status",
                        "Token",
                        "CreatedAt"
                    )
                    VALUES
                    (
                        @org,
                        @login,
                        @mail,
                        @role,
                        'Pending',
                        @token,
                        now()
                    )
                """,
                ("@org", request.LoginOrganization),
                ("@login", request.TargetLogin),
                ("@mail", targetEmail),
                ("@role", role),
                ("@token", token));

                // FIX:
                // Old links used FrontendUrl + /organization-invitation and caused:
                // Cannot GET /organization-invitation
                // New links go directly to AuthService GET endpoints.
                var authUrl = config["App:AuthServiceUrl"] ?? "http://localhost:5154";
                var acceptUrl = $"{authUrl}/organization/invitations/accept-link?token={token}";
                var declineUrl = $"{authUrl}/organization/invitations/decline-link?token={token}";

                await PublishEmailAsync(config,
                    to: targetEmail,
                    subject: "Запрошення до SportHive організації",
                    body: BuildInvitationHtml(targetName, request.LoginOrganization, role, acceptUrl, declineUrl));

                var created = (await QueryAsync(db, """
                    SELECT
                        "Id" AS "id",
                        "LoginOrganization" AS "loginOrganization",
                        "TargetLogin" AS "targetLogin",
                        "TargetEmail" AS "targetEmail",
                        "TargetRole" AS "targetRole",
                        "Status" AS "status",
                        "CreatedAt" AS "createdAt",
                        "RespondedAt" AS "respondedAt"
                    FROM "OrganizationInvitation"
                    WHERE "Token" = @token
                    LIMIT 1
                """, ("@token", token))).First();

                return Results.Ok(created);
            });

            group.MapPost("/invitations/accept", async (AppDbContext db, IConfiguration config, InviteDecisionRequest request) =>
                await ProcessInvitation(db, config, request.Token, true, false));

            group.MapPost("/invitations/decline", async (AppDbContext db, IConfiguration config, InviteDecisionRequest request) =>
                await ProcessInvitation(db, config, request.Token, false, false));

            // Browser/email links use GET, so user can click link directly from email.
            group.MapGet("/invitations/accept-link", async (AppDbContext db, IConfiguration config, string token) =>
                await ProcessInvitation(db, config, token, true, true));

            group.MapGet("/invitations/decline-link", async (AppDbContext db, IConfiguration config, string token) =>
                await ProcessInvitation(db, config, token, false, true));
        }

        private static async Task<IResult> ProcessInvitation(
            AppDbContext db,
            IConfiguration config,
            string token,
            bool accept,
            bool htmlResponse)
        {
            var invitation = await GetInvitation(db, token);

            if (invitation is null)
                return htmlResponse ? HtmlPage("Запрошення не знайдено", "Посилання недійсне або запрошення видалено.") : Results.NotFound("Invitation not found");

            var status = Convert.ToString(invitation["status"]) ?? "";
            var org = Convert.ToString(invitation["loginOrganization"]) ?? "";
            var login = Convert.ToString(invitation["targetLogin"]) ?? "";
            var role = Convert.ToString(invitation["targetRole"]) ?? "";

            if (status != "Pending")
            {
                var msg = $"Запрошення вже оброблено. Поточний статус: {status}.";
                return htmlResponse ? HtmlPage("Запрошення вже оброблено", msg) : Results.BadRequest(msg);
            }

            if (accept)
            {
                if (role == "Judge")
                {
                    await ExecuteAsync(db, """
                        INSERT INTO "OrganizationJudge" ("LoginOrganization", "LoginJudge")
                        VALUES (@org, @login)
                        ON CONFLICT DO NOTHING
                    """, ("@org", org), ("@login", login));
                }
                else if (role == "Trainer")
                {
                    await ExecuteAsync(db, """
                        INSERT INTO "OrganizationTrainer" ("LoginOrganization", "LoginTraine")
                        VALUES (@org, @login)
                        ON CONFLICT DO NOTHING
                    """, ("@org", org), ("@login", login));
                }
                else if (role == "Athlete")
                {
                    await ExecuteAsync(db, """
                        INSERT INTO "OrganizationAthlete" ("LoginOrganization", "LoginAthlete")
                        VALUES (@org, @login)
                        ON CONFLICT DO NOTHING
                    """, ("@org", org), ("@login", login));
                }

                await ExecuteAsync(db, """
                    UPDATE "OrganizationInvitation"
                    SET "Status" = 'Accepted', "RespondedAt" = now()
                    WHERE "Token" = @token
                """, ("@token", token));

                await NotifyOrganization(db, config, org,
                    "Запрошення SportHive прийнято",
                    $"Користувач {login} прийняв запрошення як {role}.");

                return htmlResponse
                    ? HtmlPage("Запрошення прийнято", "Ви успішно приєдналися до організації. Організація отримала повідомлення.")
                    : Results.Ok(new { status = "Accepted" });
            }

            await ExecuteAsync(db, """
                UPDATE "OrganizationInvitation"
                SET "Status" = 'Declined', "RespondedAt" = now()
                WHERE "Token" = @token
            """, ("@token", token));

            await NotifyOrganization(db, config, org,
                "Запрошення SportHive відхилено",
                $"Користувач {login} відхилив запрошення як {role}.");

            return htmlResponse
                ? HtmlPage("Запрошення відхилено", "Ви відхилили запрошення. Організація отримала повідомлення.")
                : Results.Ok(new { status = "Declined" });
        }

        private static IResult HtmlPage(string title, string message)
        {
            var html = $$"""
<!doctype html>
<html lang="uk">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>{{Escape(title)}}</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background: #f6f6f6;
            margin: 0;
            padding: 40px 16px;
            color: #111;
        }

        .card {
            max-width: 720px;
            margin: 0 auto;
            background: #fff;
            border: 2px solid #111;
            padding: 28px;
            box-shadow: 6px 6px 0 rgba(0,0,0,.16);
        }

        h1 { margin-top: 0; }

        a {
            display: inline-block;
            margin-top: 18px;
            background: #111;
            color: #fff;
            padding: 12px 18px;
            text-decoration: none;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <div class="card">
        <h1>{{Escape(title)}}</h1>
        <p>{{Escape(message)}}</p>
        <a href="http://localhost:3000">Повернутися в SportHive</a>
    </div>
</body>
</html>
""";

return Results.Content(html, "text/html; charset=utf-8");
        }

        private static async Task PublishEmailAsync(
            IConfiguration config,
            string to,
            string subject,
            string body)
        {
            if (string.IsNullOrWhiteSpace(to))
                return;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9093"
            };

            using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

            var from =
                config["Smtp:From"] ??
                config["Smtp:Username"] ??
                "vadimrudis7@gmail.com";

            var message = new
            {
                From = from,
                To = to,
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                Kind = "organization_invitation"
            };

            await producer.ProduceAsync("user_email", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(message)
            });
        }

        private static string BuildInvitationHtml(
            string targetName,
            string organizationLogin,
            string role,
            string acceptUrl,
            string declineUrl)
        {
            return $"""
            <div style="font-family:Arial,sans-serif;line-height:1.5;color:#111">
                <h2>Запрошення до SportHive</h2>
                <p>Вітаємо, <b>{Escape(targetName)}</b>!</p>
                <p>Організація <b>{Escape(organizationLogin)}</b> запросила вас приєднатися до SportHive як <b>{Escape(role)}</b>.</p>
                <p>
                    <a href="{acceptUrl}" style="display:inline-block;background:#111;color:#fff;padding:12px 18px;text-decoration:none;font-weight:bold;margin-right:10px">
                        Прийняти запрошення
                    </a>
                    <a href="{declineUrl}" style="display:inline-block;background:#fff;color:#111;border:2px solid #111;padding:10px 16px;text-decoration:none;font-weight:bold">
                        Відхилити
                    </a>
                </p>
                <p style="color:#666">Якщо кнопки не працюють, скопіюйте посилання:</p>
                <p>Прийняти: <br/><a href="{acceptUrl}">{acceptUrl}</a></p>
                <p>Відхилити: <br/><a href="{declineUrl}">{declineUrl}</a></p>
            </div>
            """;
        }

        private static async Task NotifyOrganization(AppDbContext db, IConfiguration config, string loginOrganization, string subject, string body)
        {
            var org = (await QueryAsync(db, """
                SELECT u."mail" AS "mail"
                FROM "Organization" o
                JOIN "user" u ON u."login" = o."Login"
                WHERE o."Login" = @org
                LIMIT 1
            """, ("@org", loginOrganization))).FirstOrDefault();

            var mail = Convert.ToString(org?.GetValueOrDefault("mail"));

            if (!string.IsNullOrWhiteSpace(mail))
                await PublishEmailAsync(config, mail, subject, $"<p>{Escape(body)}</p>");
        }

        private static string NormalizeRole(string role)
        {
            role = role.Trim();

            if (role.Equals("Athlete", StringComparison.OrdinalIgnoreCase)) return "Athlete";
            if (role.Equals("Judge", StringComparison.OrdinalIgnoreCase)) return "Judge";
            if (role.Equals("Trainer", StringComparison.OrdinalIgnoreCase)) return "Trainer";

            return role;
        }

        private static async Task<Dictionary<string, object?>?> GetInvitation(AppDbContext db, string token)
        {
            return (await QueryAsync(db, """
                SELECT
                    "LoginOrganization" AS "loginOrganization",
                    "TargetLogin" AS "targetLogin",
                    "TargetRole" AS "targetRole",
                    "TargetEmail" AS "targetEmail",
                    "Status" AS "status"
                FROM "OrganizationInvitation"
                WHERE "Token" = @token
                LIMIT 1
            """, ("@token", token))).FirstOrDefault();
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
                Add(command, name, value);

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

        private static async Task ExecuteAsync(
            AppDbContext db,
            string sql,
            params (string Name, object? Value)[] parameters)
        {
            var connection = db.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            foreach (var (name, value) in parameters)
                Add(command, name, value);

            await command.ExecuteNonQueryAsync();
        }

        private static void Add(IDbCommand command, string name, object? value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        private static string Escape(string value)
        {
            return System.Net.WebUtility.HtmlEncode(value);
        }
    }

    public sealed record OrganizationInviteRequest(
        string LoginOrganization,
        string TargetLogin,
        string TargetRole);

    public sealed record InviteDecisionRequest(string Token);
}
