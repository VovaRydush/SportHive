using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace SportHive.FinalFeatures;

public interface IFinalStatisticsService
{
    Task<StatisticsDashboardDto> GetGlobalAsync(StatisticFilter filter, CancellationToken ct = default);
    Task<StatisticsDashboardDto> GetOrganizationAsync(string organizationLogin, StatisticFilter filter, CancellationToken ct = default);
}

public sealed class FinalStatisticsService : IFinalStatisticsService
{
    private readonly AppDbContext _db;

    public FinalStatisticsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<StatisticsDashboardDto> GetGlobalAsync(StatisticFilter filter, CancellationToken ct = default)
    {
        var matches = await LoadMatchesAsync(null, filter, ct);
        return BuildDashboard(matches, filter);
    }

    public async Task<StatisticsDashboardDto> GetOrganizationAsync(string organizationLogin, StatisticFilter filter, CancellationToken ct = default)
    {
        var matches = await LoadMatchesAsync(organizationLogin, filter, ct);
        return BuildDashboard(matches, filter);
    }

    private StatisticsDashboardDto BuildDashboard(List<MatchFact> matches, StatisticFilter filter)
    {
        var finished = matches.Where(x => x.Status == 2).ToList();

        return new StatisticsDashboardDto
        {
            Filter = filter,
            Seasons = matches.Select(x => x.Season).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderByDescending(x => x).ToList(),
            Sports = matches.Select(x => x.Sport).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).ToList(),
            Summary = new StatisticSummaryDto
            {
                Events = matches.Select(x => x.EventId).Where(x => x > 0).Distinct().Count(),
                Matches = matches.Count,
                FinishedMatches = matches.Count(x => x.Status == 2),
                LiveMatches = matches.Count(x => x.Status == 1),
                UpcomingMatches = matches.Count(x => x.Status == 0),
                Participants = matches.SelectMany(x => new[] { x.FirstId, x.SecondId }).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Count(),
                Teams = matches.Where(x => x.MatchType == "team").SelectMany(x => new[] { x.FirstId, x.SecondId }).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Count(),
                Organizations = matches.SelectMany(x => new[] { x.FirstOrgLogin, x.SecondOrgLogin }).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Count()
            },
            AthleteLeaders = BuildParticipantLeaders(finished.Where(x => x.MatchType == "individual"), "Athlete", filter),
            TeamLeaders = BuildParticipantLeaders(finished.Where(x => x.MatchType == "team"), "Team", filter),
            OrganizationLeaders = BuildOrganizationLeaders(finished, filter),
            JudgeLeaders = BuildJudgeLeaders(matches, filter),
            RecentFinishedMatches = finished
                .OrderByDescending(x => x.DateMatch ?? DateTimeOffset.MinValue)
                .Take(20)
                .Select(x => new
                {
                    x.EventId,
                    x.EventName,
                    x.MatchType,
                    x.FirstName,
                    x.SecondName,
                    Score = x.ScoreText,
                    x.Winner,
                    x.Sport,
                    x.System,
                    x.Season,
                    x.DateMatch,
                    Judge = x.JudgeLogin
                })
                .Cast<object>()
                .ToList()
        };
    }

    private static List<LeaderboardRowDto> BuildParticipantLeaders(IEnumerable<MatchFact> matches, string type, StatisticFilter filter)
    {
        var map = new Dictionary<string, LeaderboardRowDto>(StringComparer.OrdinalIgnoreCase);

        foreach (var match in matches)
        {
            var first = EnsureRow(map, match.FirstId, match.FirstName, type, match.Sport, match.FirstOrgLogin, match.FirstOrgName);
            var second = EnsureRow(map, match.SecondId, match.SecondName, type, match.Sport, match.SecondOrgLogin, match.SecondOrgName);
            ApplyResult(first, second, match);
        }

        return SortRows(map.Values, filter).Take(100).ToList();
    }

    private static List<LeaderboardRowDto> BuildOrganizationLeaders(IEnumerable<MatchFact> matches, StatisticFilter filter)
    {
        var map = new Dictionary<string, LeaderboardRowDto>(StringComparer.OrdinalIgnoreCase);

        foreach (var match in matches)
        {
            if (string.IsNullOrWhiteSpace(match.FirstOrgLogin) || string.IsNullOrWhiteSpace(match.SecondOrgLogin))
                continue;

            var first = EnsureRow(map, match.FirstOrgLogin, match.FirstOrgName ?? match.FirstOrgLogin, "Organization", match.Sport, match.FirstOrgLogin, match.FirstOrgName);
            var second = EnsureRow(map, match.SecondOrgLogin, match.SecondOrgName ?? match.SecondOrgLogin, "Organization", match.Sport, match.SecondOrgLogin, match.SecondOrgName);
            ApplyResult(first, second, match);
        }

        return SortRows(map.Values, filter).Take(100).ToList();
    }

    private static List<LeaderboardRowDto> BuildJudgeLeaders(IEnumerable<MatchFact> matches, StatisticFilter filter)
    {
        var rows = matches
            .Where(x => !string.IsNullOrWhiteSpace(x.JudgeLogin))
            .GroupBy(x => x.JudgeLogin!, StringComparer.OrdinalIgnoreCase)
            .Select(g => new LeaderboardRowDto
            {
                Id = g.Key,
                Name = g.Key,
                Type = "Judge",
                Sport = g.FirstOrDefault()?.Sport,
                Played = g.Count(),
                Finished = g.Count(x => x.Status == 2),
                Points = g.Count(x => x.Status == 2)
            });

        return SortRows(rows, filter).Take(100).ToList();
    }

    private static LeaderboardRowDto EnsureRow(Dictionary<string, LeaderboardRowDto> map, string id, string? name, string type, string? sport, string? orgLogin, string? orgName)
    {
        id = string.IsNullOrWhiteSpace(id) ? (name ?? "") : id;
        name = string.IsNullOrWhiteSpace(name) ? id : name;

        if (!map.TryGetValue(id, out var row))
        {
            row = new LeaderboardRowDto
            {
                Id = id,
                Name = name ?? id,
                Type = type,
                Sport = sport,
                OrganizationLogin = orgLogin,
                OrganizationName = orgName
            };
            map[id] = row;
        }

        return row;
    }

    private static void ApplyResult(LeaderboardRowDto first, LeaderboardRowDto second, MatchFact match)
    {
        first.Played++;
        second.Played++;
        first.Finished++;
        second.Finished++;

        first.ScoreFor += match.FirstScore;
        first.ScoreAgainst += match.SecondScore;
        second.ScoreFor += match.SecondScore;
        second.ScoreAgainst += match.FirstScore;

        first.ScoreDiff = first.ScoreFor - first.ScoreAgainst;
        second.ScoreDiff = second.ScoreFor - second.ScoreAgainst;

        if (match.FirstScore > match.SecondScore)
        {
            first.Wins++;
            first.Points += 3;
            second.Losses++;
        }
        else if (match.SecondScore > match.FirstScore)
        {
            second.Wins++;
            second.Points += 3;
            first.Losses++;
        }
        else if (!string.IsNullOrWhiteSpace(match.Winner))
        {
            if (IsWinner(match.Winner, match.FirstId, match.FirstName))
            {
                first.Wins++;
                first.Points += 3;
                second.Losses++;
            }
            else if (IsWinner(match.Winner, match.SecondId, match.SecondName))
            {
                second.Wins++;
                second.Points += 3;
                first.Losses++;
            }
            else
            {
                first.Draws++;
                second.Draws++;
                first.Points++;
                second.Points++;
            }
        }
        else
        {
            first.Draws++;
            second.Draws++;
            first.Points++;
            second.Points++;
        }

        first.WinRate = first.Finished == 0 ? 0 : Math.Round(first.Wins * 100.0 / first.Finished, 1);
        second.WinRate = second.Finished == 0 ? 0 : Math.Round(second.Wins * 100.0 / second.Finished, 1);
    }

    private static bool IsWinner(string winner, string id, string? name)
    {
        return string.Equals(winner, id, StringComparison.OrdinalIgnoreCase)
            || string.Equals(winner, name, StringComparison.OrdinalIgnoreCase);
    }

    private static IEnumerable<LeaderboardRowDto> SortRows(IEnumerable<LeaderboardRowDto> rows, StatisticFilter filter)
    {
        var sort = (filter.SortBy ?? "points").Trim().ToLowerInvariant();
        var desc = !string.Equals(filter.Direction, "asc", StringComparison.OrdinalIgnoreCase);

        Func<LeaderboardRowDto, object> key = sort switch
        {
            "played" => r => r.Played,
            "wins" => r => r.Wins,
            "winrate" => r => r.WinRate,
            "scorediff" => r => r.ScoreDiff,
            "scorefor" => r => r.ScoreFor,
            "name" => r => r.Name,
            _ => r => r.Points
        };

        return desc ? rows.OrderByDescending(key).ThenBy(r => r.Name) : rows.OrderBy(key).ThenBy(r => r.Name);
    }

    private async Task<List<MatchFact>> LoadMatchesAsync(string? organizationLogin, StatisticFilter filter, CancellationToken ct)
    {
        var result = new List<MatchFact>();

        result.AddRange(await QueryIndividualAsync(organizationLogin, filter, ct));
        result.AddRange(await QueryTeamAsync(organizationLogin, filter, ct));

        return result;
    }

    private async Task<List<MatchFact>> QueryIndividualAsync(string? organizationLogin, StatisticFilter filter, CancellationToken ct)
    {
        var rows = await QueryAsync("""
            SELECT
                im."IdIndividualMatch" AS match_id,
                im."IdEvent" AS event_id,
                e."NameEvent" AS event_name,
                e."TypeSport" AS sport,
                e."systems" AS system_name,
                EXTRACT(YEAR FROM e."DataStart")::text AS season,
                im."loginFirstAthlete" AS first_id,
                CONCAT(a1."FirsName", ' ', a1."LastName") AS first_name,
                im."loginSecondAthlete" AS second_id,
                CONCAT(a2."FirsName", ' ', a2."LastName") AS second_name,
                o1."Login" AS first_org_login,
                o1."NameOrganization" AS first_org_name,
                o2."Login" AS second_org_login,
                o2."NameOrganization" AS second_org_name,
                im."loginJudge" AS judge_login,
                im."DataMatch" AS date_match,
                im."AddInformation" AS add_information,
                im."StatusMatch" AS status_match
            FROM "IndividualMatch" im
            LEFT JOIN "Event" e ON e."IdEvent" = im."IdEvent"
            LEFT JOIN "Athlete" a1 ON a1."Login" = im."loginFirstAthlete"
            LEFT JOIN "Athlete" a2 ON a2."Login" = im."loginSecondAthlete"
            LEFT JOIN "OrganizationAthlete" oa1 ON oa1."LoginAthlete" = im."loginFirstAthlete"
            LEFT JOIN "Organization" o1 ON o1."Login" = oa1."LoginOrganization"
            LEFT JOIN "OrganizationAthlete" oa2 ON oa2."LoginAthlete" = im."loginSecondAthlete"
            LEFT JOIN "Organization" o2 ON o2."Login" = oa2."LoginOrganization"
            WHERE (@season IS NULL OR EXTRACT(YEAR FROM e."DataStart")::text = @season)
              AND (@sport IS NULL OR e."TypeSport" = @sport)
              AND (@system IS NULL OR e."systems" = @system)
              AND (@org IS NULL OR o1."Login" = @org OR o2."Login" = @org)
        """, filter, organizationLogin, ct);

        return rows.Select(r => ToMatchFact(r, "individual")).ToList();
    }

    private async Task<List<MatchFact>> QueryTeamAsync(string? organizationLogin, StatisticFilter filter, CancellationToken ct)
    {
        var rows = await QueryAsync("""
            SELECT
                tm."IdTeamMatch" AS match_id,
                tm."IdEvent" AS event_id,
                e."NameEvent" AS event_name,
                e."TypeSport" AS sport,
                e."systems" AS system_name,
                EXTRACT(YEAR FROM e."DataStart")::text AS season,
                tm."NameFirstTeam" AS first_id,
                tm."NameFirstTeam" AS first_name,
                tm."NameSecondTeam" AS second_id,
                tm."NameSecondTeam" AS second_name,
                o1."Login" AS first_org_login,
                o1."NameOrganization" AS first_org_name,
                o2."Login" AS second_org_login,
                o2."NameOrganization" AS second_org_name,
                tm."loginJudge" AS judge_login,
                tm."DataMatch" AS date_match,
                tm."AddInformation" AS add_information,
                tm."StatusMatch" AS status_match
            FROM "TeamMatch" tm
            LEFT JOIN "Event" e ON e."IdEvent" = tm."IdEvent"
            LEFT JOIN "OrganizationTeam" ot1 ON ot1."NameComand" = tm."NameFirstTeam"
            LEFT JOIN "Organization" o1 ON o1."Login" = ot1."LoginOrganization"
            LEFT JOIN "OrganizationTeam" ot2 ON ot2."NameComand" = tm."NameSecondTeam"
            LEFT JOIN "Organization" o2 ON o2."Login" = ot2."LoginOrganization"
            WHERE (@season IS NULL OR EXTRACT(YEAR FROM e."DataStart")::text = @season)
              AND (@sport IS NULL OR e."TypeSport" = @sport)
              AND (@system IS NULL OR e."systems" = @system)
              AND (@org IS NULL OR o1."Login" = @org OR o2."Login" = @org)
        """, filter, organizationLogin, ct);

        return rows.Select(r => ToMatchFact(r, "team")).ToList();
    }

    private async Task<List<Dictionary<string, object?>>> QueryAsync(string sql, StatisticFilter filter, string? organizationLogin, CancellationToken ct)
    {
        var result = new List<Dictionary<string, object?>>();
        var connection = _db.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        AddTextParameter(command, "@season", NormalizeFilter(filter.Season));
        AddTextParameter(command, "@sport", NormalizeFilter(filter.Sport));
        AddTextParameter(command, "@system", NormalizeFilter(filter.System));
        AddTextParameter(command, "@org", NormalizeFilter(organizationLogin));

        await using var reader = await command.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            var row = new Dictionary<string, object?>();

            for (var i = 0; i < reader.FieldCount; i++)
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);

            result.Add(row);
        }

        return result;
    }

    private static object NormalizeFilter(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Equals("all", StringComparison.OrdinalIgnoreCase))
            return DBNull.Value;

        return value;
    }

    private static void AddTextParameter(IDbCommand command, string name, object value)
    {
        var p = command.CreateParameter();
        p.ParameterName = name;
        p.DbType = DbType.String;
        p.Value = value;
        command.Parameters.Add(p);
    }

    private static MatchFact ToMatchFact(Dictionary<string, object?> row, string matchType)
    {
        var addInfo = Convert.ToString(row.GetValueOrDefault("add_information")) ?? "";
        var score = ExtractScore(addInfo);
        var (s1, s2) = ParseScore(score);
        var winner = ExtractValue(addInfo, "winner");
        var status = Convert.ToInt32(row.GetValueOrDefault("status_match") ?? 0);

        if (status != 2 && (addInfo.Contains("winner=", StringComparison.OrdinalIgnoreCase)
            || addInfo.Contains("finishedAt=", StringComparison.OrdinalIgnoreCase)
            || addInfo.Contains("matchFinished=true", StringComparison.OrdinalIgnoreCase)
            || addInfo.Contains("status=Finished", StringComparison.OrdinalIgnoreCase)))
        {
            status = 2;
        }

        return new MatchFact
        {
            MatchType = matchType,
            MatchId = Convert.ToInt64(row.GetValueOrDefault("match_id") ?? 0),
            EventId = Convert.ToInt64(row.GetValueOrDefault("event_id") ?? 0),
            EventName = Convert.ToString(row.GetValueOrDefault("event_name")) ?? "",
            Sport = Convert.ToString(row.GetValueOrDefault("sport")) ?? "",
            System = Convert.ToString(row.GetValueOrDefault("system_name")) ?? "",
            Season = Convert.ToString(row.GetValueOrDefault("season")) ?? "",
            FirstId = Convert.ToString(row.GetValueOrDefault("first_id")) ?? "",
            FirstName = Convert.ToString(row.GetValueOrDefault("first_name")) ?? "",
            SecondId = Convert.ToString(row.GetValueOrDefault("second_id")) ?? "",
            SecondName = Convert.ToString(row.GetValueOrDefault("second_name")) ?? "",
            FirstOrgLogin = Convert.ToString(row.GetValueOrDefault("first_org_login")),
            FirstOrgName = Convert.ToString(row.GetValueOrDefault("first_org_name")),
            SecondOrgLogin = Convert.ToString(row.GetValueOrDefault("second_org_login")),
            SecondOrgName = Convert.ToString(row.GetValueOrDefault("second_org_name")),
            JudgeLogin = Convert.ToString(row.GetValueOrDefault("judge_login")),
            DateMatch = ConvertDate(row.GetValueOrDefault("date_match")),
            AddInformation = addInfo,
            ScoreText = score,
            Winner = winner,
            FirstScore = s1,
            SecondScore = s2,
            Status = status
        };
    }

    private static DateTimeOffset? ConvertDate(object? value)
    {
        if (value is null) return null;
        if (value is DateTimeOffset dto) return dto;
        if (value is DateTime dt) return new DateTimeOffset(dt);
        if (DateTimeOffset.TryParse(Convert.ToString(value), out var parsed)) return parsed;
        return null;
    }

    private static string ExtractScore(string addInfo)
    {
        var match = System.Text.RegularExpressions.Regex.Match(addInfo ?? "", @"score\s*=\s*(\d+\s*[:\-]\s*\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Replace(" ", "").Replace("-", ":") : "";
    }

    private static string ExtractValue(string addInfo, string key)
    {
        var match = System.Text.RegularExpressions.Regex.Match(addInfo ?? "", @$"{key}\s*=\s*([^;]+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : "";
    }

    private static (int, int) ParseScore(string score)
    {
        var parts = (score ?? "").Split(':');
        if (parts.Length == 2 && int.TryParse(parts[0], out var a) && int.TryParse(parts[1], out var b))
            return (a, b);

        return (0, 0);
    }

    private sealed class MatchFact
    {
        public string MatchType { get; set; } = "";
        public long MatchId { get; set; }
        public long EventId { get; set; }
        public string EventName { get; set; } = "";
        public string Sport { get; set; } = "";
        public string System { get; set; } = "";
        public string Season { get; set; } = "";
        public string FirstId { get; set; } = "";
        public string? FirstName { get; set; }
        public string SecondId { get; set; } = "";
        public string? SecondName { get; set; }
        public string? FirstOrgLogin { get; set; }
        public string? FirstOrgName { get; set; }
        public string? SecondOrgLogin { get; set; }
        public string? SecondOrgName { get; set; }
        public string? JudgeLogin { get; set; }
        public DateTimeOffset? DateMatch { get; set; }
        public string AddInformation { get; set; } = "";
        public string ScoreText { get; set; } = "";
        public string Winner { get; set; } = "";
        public int FirstScore { get; set; }
        public int SecondScore { get; set; }
        public int Status { get; set; }
    }
}
