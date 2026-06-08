using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;
using System.Text.Json;
using Confluent.Kafka;

namespace Events.Endpoints
{
    public static class EventInvitationEndpoints
    {
        public static void EventInvitationEndpoint(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/event-invitations");

            group.MapGet("/search-participants", async (
                AppDbContext db,
                string participantType,
                string typeSport,
                string? query) =>
            {
                participantType = NormalizeParticipantType(participantType);
                query ??= "";

                if (participantType == "team")
                {
                    var teams = await QueryAsync(db, """
                        SELECT DISTINCT ON (t."TeamName")
                            t."TeamName" AS "id",
                            t."TeamName" AS "name",
                            'team' AS "type",
                            CONCAT('Команда · ', t."TypeSport", ' · організація: ', COALESCE(ot."LoginOrganization", '-')) AS "subtitle",
                            ou."mail" AS "email",
                            ot."LoginOrganization" AS "ownerOrganizationLogin"
                        FROM "Team" t
                        LEFT JOIN "OrganizationTeam" ot ON ot."NameComand" = t."TeamName"
                        LEFT JOIN "Organization" o ON o."Login" = ot."LoginOrganization"
                        LEFT JOIN "user" ou ON ou."login" = o."Login"
                        WHERE t."TypeSport" ILIKE @sport
                          AND (
                            @q = ''
                            OR t."TeamName" ILIKE @like
                            OR t."LoginTrainer" ILIKE @like
                          )
                          AND ou."mail" IS NOT NULL
                        ORDER BY t."TeamName"
                        LIMIT 50
                    """, ("@sport", typeSport), ("@q", query), ("@like", $"%{query}%"));

                    return Results.Ok(teams);
                }

                var athletes = await QueryAsync(db, """
                    SELECT
                        a."Login" AS "id",
                        CONCAT(a."FirsName", ' ', a."LastName") AS "name",
                        'individual' AS "type",
                        CONCAT('Спортсмен · ', a."TypeSport", ' · login: ', a."Login") AS "subtitle",
                        u."mail" AS "email",
                        NULL AS "ownerOrganizationLogin"
                    FROM "Athlete" a
                    JOIN "user" u ON u."login" = a."Login"
                    WHERE a."TypeSport" ILIKE @sport
                      AND (
                        @q = ''
                        OR a."Login" ILIKE @like
                        OR a."FirsName" ILIKE @like
                        OR a."LastName" ILIKE @like
                        OR CONCAT(a."FirsName", ' ', a."LastName") ILIKE @like
                        OR u."mail" ILIKE @like
                      )
                    ORDER BY a."LastName", a."FirsName"
                    LIMIT 50
                """, ("@sport", typeSport), ("@q", query), ("@like", $"%{query}%"));

                return Results.Ok(athletes);
            });

            group.MapPost("/create", async (
                AppDbContext db,
                IConfiguration config,
                CreateEventInvitationRequest request) =>
            {
                var participantType = NormalizeParticipantType(request.ParticipantType);
                var targets = await ResolveTargets(db, participantType, request.TypeSport, request.Participants);

                if (targets.Count < 2)
                    return Results.BadRequest("Потрібно мінімум 2 валідні учасники з email.");

                var deadline = DateTime.UtcNow.AddHours(24);

                var batchId = await ExecuteScalarLongAsync(db, """
                    INSERT INTO "EventInvitationBatch"
                    (
                        "NameEvent",
                        "TypeSport",
                        "Systems",
                        "ParticipantType",
                        "DataStart",
                        "DataEnd",
                        "Description",
                        "LoginJudge",
                        "CreatedByOrganization",
                        "Status",
                        "DeadlineAt",
                        "CreatedAt"
                    )
                    VALUES
                    (
                        @name,
                        @sport,
                        @systems,
                        @ptype,
                        @start,
                        @end,
                        @description,
                        @judge,
                        @org,
                        'Pending',
                        @deadline,
                        now()
                    )
                    RETURNING "Id"
                """,
                ("@name", request.NameEvent),
                ("@sport", request.TypeSport),
                ("@systems", request.Systems.ToString()),
                ("@ptype", participantType),
                ("@start", request.DataStart.ToUniversalTime()),
                ("@end", request.DataEnd?.ToUniversalTime()),
                ("@description", request.Description ?? ""),
                ("@judge", string.IsNullOrWhiteSpace(request.LoginJudge) ? null : request.LoginJudge),
                ("@org", string.IsNullOrWhiteSpace(request.CreatedByOrganization) ? null : request.CreatedByOrganization),
                ("@deadline", deadline));

                foreach (var target in targets)
                {
                    var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();

                    await ExecuteAsync(db, """
                        INSERT INTO "EventParticipationInvitation"
                        (
                            "BatchId",
                            "TargetType",
                            "TargetLoginOrName",
                            "TargetDisplayName",
                            "TargetEmail",
                            "OwnerOrganizationLogin",
                            "Status",
                            "Token",
                            "CreatedAt"
                        )
                        VALUES
                        (
                            @batch,
                            @targetType,
                            @targetId,
                            @display,
                            @email,
                            @owner,
                            'Pending',
                            @token,
                            now()
                        )
                    """,
                    ("@batch", batchId),
                    ("@targetType", participantType),
                    ("@targetId", target.Id),
                    ("@display", target.Name),
                    ("@email", target.Email),
                    ("@owner", target.OwnerOrganizationLogin),
                    ("@token", token));

                    var eventUrl = config["App:EventServiceUrl"] ?? "http://localhost:5042";
                    var acceptUrl = $"{eventUrl}/event-invitations/accept-link?token={token}";
                    var declineUrl = $"{eventUrl}/event-invitations/decline-link?token={token}";

                    await PublishEmailAsync(config, target.Email,
                        $"Запрошення на участь у заході SportHive: {request.NameEvent}",
                        BuildInvitationHtml(target.Name, request.NameEvent, request.TypeSport, deadline, acceptUrl, declineUrl));
                }

                var required = RequiredCount(targets.Count);

                return Results.Ok(new
                {
                    batchId,
                    deadlineAt = deadline,
                    totalInvited = targets.Count,
                    acceptedCount = 0,
                    requiredCount = required,
                    status = "Pending"
                });
            });

            group.MapGet("/accept-link", async (AppDbContext db, IConfiguration config, string token) =>
            {
                var result = await ProcessDecision(db, config, token, accept: true);
                return HtmlPage(result.Title, result.Message);
            });

            group.MapGet("/decline-link", async (AppDbContext db, IConfiguration config, string token) =>
            {
                var result = await ProcessDecision(db, config, token, accept: false);
                return HtmlPage(result.Title, result.Message);
            });

            group.MapPost("/accept", async (AppDbContext db, IConfiguration config, InvitationDecisionRequest request) =>
            {
                var result = await ProcessDecision(db, config, request.Token, accept: true);
                return Results.Ok(result);
            });

            group.MapPost("/decline", async (AppDbContext db, IConfiguration config, InvitationDecisionRequest request) =>
            {
                var result = await ProcessDecision(db, config, request.Token, accept: false);
                return Results.Ok(result);
            });
        }

        public static async Task<DecisionResult> ProcessDecision(AppDbContext db, IConfiguration config, string token, bool accept)
        {
            var invitation = (await QueryAsync(db, """
                SELECT
                    i."Id" AS "id",
                    i."BatchId" AS "batchId",
                    i."Status" AS "status",
                    i."TargetEmail" AS "targetEmail",
                    b."Status" AS "batchStatus",
                    b."DeadlineAt" AS "deadlineAt",
                    b."NameEvent" AS "nameEvent"
                FROM "EventParticipationInvitation" i
                JOIN "EventInvitationBatch" b ON b."Id" = i."BatchId"
                WHERE i."Token" = @token
                LIMIT 1
            """, ("@token", token))).FirstOrDefault();

            if (invitation is null)
                return new DecisionResult("Запрошення не знайдено", "Посилання недійсне або запрошення видалено.");

            var batchStatus = Convert.ToString(invitation["batchStatus"]);
            var status = Convert.ToString(invitation["status"]);
            var batchId = Convert.ToInt64(invitation["batchId"]);
            var deadline = Convert.ToDateTime(invitation["deadlineAt"]).ToUniversalTime();

            if (batchStatus != "Pending")
                return new DecisionResult("Захід вже оброблено", $"Поточний статус заходу: {batchStatus}.");

            if (DateTime.UtcNow > deadline)
                return new DecisionResult("Час вийшов", "24 години на підтвердження вже минули.");

            if (status != "Pending")
                return new DecisionResult("Запрошення вже оброблено", $"Поточний статус: {status}.");

            await ExecuteAsync(db, """
                UPDATE "EventParticipationInvitation"
                SET "Status" = @status, "RespondedAt" = now()
                WHERE "Token" = @token
            """, ("@status", accept ? "Accepted" : "Declined"), ("@token", token));

            if (accept)
            {
                await TryCreateEventIfQuorumReached(db, config, batchId);
                return new DecisionResult("Участь підтверджено", "Ваша відповідь прийнята. Якщо 70% учасників підтвердять участь, захід буде створено автоматично.");
            }

            return new DecisionResult("Участь відхилено", "Ваша відповідь прийнята.");
        }

        public static async Task CheckExpiredBatches(AppDbContext db, IConfiguration config)
        {
            var batches = await QueryAsync(db, """
                SELECT "Id" AS "id"
                FROM "EventInvitationBatch"
                WHERE "Status" = 'Pending'
                  AND "DeadlineAt" <= now()
            """);

            foreach (var batch in batches)
            {
                var batchId = Convert.ToInt64(batch["id"]);
                var stats = await GetBatchStats(db, batchId);

                if (stats.Accepted >= stats.Required)
                {
                    await CreateEventFromBatch(db, config, batchId);
                }
                else
                {
                    await ExecuteAsync(db, """
                        UPDATE "EventInvitationBatch"
                        SET "Status" = 'Cancelled', "UpdatedAt" = now()
                        WHERE "Id" = @batch
                    """, ("@batch", batchId));

                    await ExecuteAsync(db, """
                        UPDATE "EventParticipationInvitation"
                        SET "Status" = 'Expired'
                        WHERE "BatchId" = @batch
                          AND "Status" = 'Pending'
                    """, ("@batch", batchId));

                    await NotifyAll(db, config, batchId,
                        "Захід SportHive не відбудеться",
                        "Захід не відбудеться, тому що менше 70% запрошених учасників підтвердили участь протягом 24 годин.");
                }
            }
        }

        private static async Task TryCreateEventIfQuorumReached(AppDbContext db, IConfiguration config, long batchId)
        {
            var stats = await GetBatchStats(db, batchId);

            if (stats.Accepted >= stats.Required)
                await CreateEventFromBatch(db, config, batchId);
        }

        private static async Task CreateEventFromBatch(AppDbContext db, IConfiguration config, long batchId)
        {
            var batch = (await QueryAsync(db, """
                SELECT *
                FROM "EventInvitationBatch"
                WHERE "Id" = @batch
                LIMIT 1
            """, ("@batch", batchId))).FirstOrDefault();

            if (batch is null)
                return;

            if (Convert.ToString(batch["Status"]) != "Pending")
                return;

            var participantType = Convert.ToString(batch["ParticipantType"]) ?? "team";
            var accepted = await QueryAsync(db, """
                SELECT
                    "TargetLoginOrName" AS "id"
                FROM "EventParticipationInvitation"
                WHERE "BatchId" = @batch
                  AND "Status" = 'Accepted'
                ORDER BY "Id"
            """, ("@batch", batchId));

            if (accepted.Count < 2)
                return;

            var eventId = await ExecuteScalarLongAsync(db, """
                INSERT INTO "Event"
                (
                    "NameEvent",
                    systems,
                    "DataStart",
                    "DataEnd",
                    description,
                    "EventPhoto",
                    "TypeSport"
                )
                VALUES
                (
                    @name,
                    @systems,
                    @start,
                    @end,
                    @description,
                    '',
                    @sport
                )
                RETURNING "IdEvent"
            """,
            ("@name", batch["NameEvent"]),
            ("@systems", batch["Systems"]),
            ("@start", batch["DataStart"]),
            ("@end", batch["DataEnd"]),
            ("@description", batch["Description"] ?? ""),
            ("@sport", batch["TypeSport"]));

            for (var i = 0; i < accepted.Count; i += 2)
            {
                if (i + 1 >= accepted.Count)
                    break;

                var first = Convert.ToString(accepted[i]["id"]) ?? "";
                var second = Convert.ToString(accepted[i + 1]["id"]) ?? "";

                if (participantType == "team")
                {
                    await ExecuteAsync(db, """
                        INSERT INTO "TeamMatch"
                        (
                            "IdEvent",
                            "NameFirstTeam",
                            "NameSecondTeam",
                            "DataMatch",
                            "TimeMatch",
                            "Tour",
                            "AddInformation",
                            "LocationName",
                            "StatusMatch",
                            "Group",
                            "loginJudge"
                        )
                        VALUES
                        (
                            @event,
                            @first,
                            @second,
                            @date,
                            @time,
                            1,
                            '',
                            NULL,
                            0,
                            1,
                            @judge
                        )
                    """,
                    ("@event", eventId),
                    ("@first", first),
                    ("@second", second),
                    ("@date", Convert.ToDateTime(batch["DataStart"]).Date),
                    ("@time", TimeSpan.Zero),
                    ("@judge", batch["LoginJudge"]));
                }
                else
                {
                    await ExecuteAsync(db, """
                        INSERT INTO "IndividualMatch"
                        (
                            "IdEvent",
                            "loginFirstAthlete",
                            "loginSecondAthlete",
                            "DataMatch",
                            "TimeMatch",
                            "Tour",
                            "AddInformation",
                            "LocationName",
                            "StatusMatch",
                            "Group",
                            "loginJudge"
                        )
                        VALUES
                        (
                            @event,
                            @first,
                            @second,
                            @date,
                            @time,
                            1,
                            '',
                            NULL,
                            0,
                            1,
                            @judge
                        )
                    """,
                    ("@event", eventId),
                    ("@first", first),
                    ("@second", second),
                    ("@date", Convert.ToDateTime(batch["DataStart"]).ToUniversalTime()),
                    ("@time", TimeSpan.Zero),
                    ("@judge", batch["LoginJudge"]));
                }
            }

            await ExecuteAsync(db, """
                UPDATE "EventInvitationBatch"
                SET "Status" = 'Created',
                    "CreatedEventId" = @event,
                    "UpdatedAt" = now()
                WHERE "Id" = @batch
            """, ("@event", eventId), ("@batch", batchId));

            await ExecuteAsync(db, """
                UPDATE "EventParticipationInvitation"
                SET "Status" = 'Expired'
                WHERE "BatchId" = @batch
                  AND "Status" = 'Pending'
            """, ("@batch", batchId));

            await NotifyAll(db, config, batchId,
                "Захід SportHive створено",
                $"70% або більше учасників підтвердили участь. Захід «{batch["NameEvent"]}» створено.");
        }

        private static async Task<BatchStats> GetBatchStats(AppDbContext db, long batchId)
        {
            var row = (await QueryAsync(db, """
                SELECT
                    COUNT(*) AS total,
                    SUM(CASE WHEN "Status" = 'Accepted' THEN 1 ELSE 0 END) AS accepted
                FROM "EventParticipationInvitation"
                WHERE "BatchId" = @batch
            """, ("@batch", batchId))).First();

            var total = Convert.ToInt32(row["total"]);
            var accepted = Convert.ToInt32(row["accepted"]);
            return new BatchStats(total, accepted, RequiredCount(total));
        }

        private static int RequiredCount(int total) => (int)Math.Ceiling(total * 0.7);

        private static async Task<List<TargetInfo>> ResolveTargets(
            AppDbContext db,
            string participantType,
            string typeSport,
            List<string> participants)
        {
            var result = new List<TargetInfo>();

            foreach (var id in participants.Distinct())
            {
                if (participantType == "team")
                {
                    var target = (await QueryAsync(db, """
                        SELECT DISTINCT ON (t."TeamName")
                            t."TeamName" AS "id",
                            t."TeamName" AS "name",
                            ou."mail" AS "email",
                            ot."LoginOrganization" AS "ownerOrganizationLogin"
                        FROM "Team" t
                        JOIN "OrganizationTeam" ot ON ot."NameComand" = t."TeamName"
                        JOIN "Organization" o ON o."Login" = ot."LoginOrganization"
                        JOIN "user" ou ON ou."login" = o."Login"
                        WHERE t."TeamName" = @id
                          AND t."TypeSport" ILIKE @sport
                          AND ou."mail" IS NOT NULL
                        ORDER BY t."TeamName"
                        LIMIT 1
                    """, ("@id", id), ("@sport", typeSport))).FirstOrDefault();

                    if (target is not null)
                    {
                        result.Add(new TargetInfo(
                            Convert.ToString(target["id"]) ?? "",
                            Convert.ToString(target["name"]) ?? "",
                            Convert.ToString(target["email"]) ?? "",
                            Convert.ToString(target["ownerOrganizationLogin"])));
                    }
                }
                else
                {
                    var target = (await QueryAsync(db, """
                        SELECT
                            a."Login" AS "id",
                            CONCAT(a."FirsName", ' ', a."LastName") AS "name",
                            u."mail" AS "email"
                        FROM "Athlete" a
                        JOIN "user" u ON u."login" = a."Login"
                        WHERE a."Login" = @id
                          AND a."TypeSport" ILIKE @sport
                          AND u."mail" IS NOT NULL
                        LIMIT 1
                    """, ("@id", id), ("@sport", typeSport))).FirstOrDefault();

                    if (target is not null)
                    {
                        result.Add(new TargetInfo(
                            Convert.ToString(target["id"]) ?? "",
                            Convert.ToString(target["name"]) ?? "",
                            Convert.ToString(target["email"]) ?? "",
                            null));
                    }
                }
            }

            return result;
        }

        private static async Task NotifyAll(AppDbContext db, IConfiguration config, long batchId, string subject, string message)
        {
            var emails = await QueryAsync(db, """
                SELECT DISTINCT "TargetEmail" AS email
                FROM "EventParticipationInvitation"
                WHERE "BatchId" = @batch
                  AND "TargetEmail" IS NOT NULL
            """, ("@batch", batchId));

            foreach (var item in emails)
            {
                var email = Convert.ToString(item["email"]);
                if (!string.IsNullOrWhiteSpace(email))
                    await PublishEmailAsync(config, email, subject, $"<p>{Escape(message)}</p>");
            }
        }

        private static async Task PublishEmailAsync(IConfiguration config, string to, string subject, string body)
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

            var payload = new
            {
                From = from,
                To = to,
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                Kind = "event_participation_invitation"
            };

            await producer.ProduceAsync("user_email", new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(payload)
            });
        }

        private static string BuildInvitationHtml(
            string targetName,
            string eventName,
            string sport,
            DateTime deadline,
            string acceptUrl,
            string declineUrl)
        {
            return $"""
            <div style="font-family:Arial,sans-serif;line-height:1.5;color:#111">
                <h2>Запрошення на участь у заході SportHive</h2>
                <p>Вітаємо, <b>{Escape(targetName)}</b>!</p>
                <p>Вас запросили взяти участь у заході <b>{Escape(eventName)}</b>.</p>
                <p>Вид спорту: <b>{Escape(sport)}</b></p>
                <p>На відповідь є 24 години. Кінцевий час: <b>{deadline:dd.MM.yyyy HH:mm} UTC</b>.</p>
                <p>Захід буде створено автоматично, якщо 70% або більше запрошених підтвердять участь.</p>
                <p>
                    <a href="{acceptUrl}" style="display:inline-block;background:#111;color:#fff;padding:12px 18px;text-decoration:none;font-weight:bold;margin-right:10px">
                        Прийняти участь
                    </a>
                    <a href="{declineUrl}" style="display:inline-block;background:#fff;color:#111;border:2px solid #111;padding:10px 16px;text-decoration:none;font-weight:bold">
                        Відхилити
                    </a>
                </p>
                <p style="color:#666">Якщо кнопки не працюють:</p>
                <p>Прийняти: <br/><a href="{acceptUrl}">{acceptUrl}</a></p>
                <p>Відхилити: <br/><a href="{declineUrl}">{declineUrl}</a></p>
            </div>
            """;
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
        body { font-family: Arial, sans-serif; background:#f6f6f6; padding:40px 16px; }
        .card { max-width:720px; margin:0 auto; background:#fff; border:2px solid #111; padding:28px; box-shadow:6px 6px 0 rgba(0,0,0,.16); }
        a { display:inline-block; margin-top:18px; background:#111; color:#fff; padding:12px 18px; text-decoration:none; font-weight:bold; }
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

        private static string NormalizeParticipantType(string value)
        {
            if (value.Equals("individual", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("athlete", StringComparison.OrdinalIgnoreCase))
                return "individual";

            return "team";
        }

        private static string Escape(object? value) => System.Net.WebUtility.HtmlEncode(Convert.ToString(value) ?? "");

        private static async Task<long> ExecuteScalarLongAsync(AppDbContext db, string sql, params (string Name, object? Value)[] parameters)
        {
            var connection = db.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            foreach (var (name, value) in parameters)
                Add(command, name, value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        private static async Task ExecuteAsync(AppDbContext db, string sql, params (string Name, object? Value)[] parameters)
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

        private static async Task<List<Dictionary<string, object?>>> QueryAsync(AppDbContext db, string sql, params (string Name, object? Value)[] parameters)
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

        private static void Add(IDbCommand command, string name, object? value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        private sealed record TargetInfo(string Id, string Name, string Email, string? OwnerOrganizationLogin);
        private sealed record BatchStats(int Total, int Accepted, int Required);
    }

    public sealed record CreateEventInvitationRequest(
        string NameEvent,
        string TypeSport,
        object Systems,
        string ParticipantType,
        List<string> Participants,
        DateTime DataStart,
        DateTime? DataEnd,
        string? Description,
        string? LoginJudge,
        string? CreatedByOrganization);

    public sealed record InvitationDecisionRequest(string Token);

    public sealed record DecisionResult(string Title, string Message);
}
