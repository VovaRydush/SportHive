using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SportHive.Exceptions;
using SportHive.Hubs;
using SportHive.Services.Interfaces;
using System.Data;
using System.Text.RegularExpressions;

namespace SportHive.Implementations.SportRules
{
    public class SportRulesService : ISportRulesService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<MatchLiveHub> _hub;
        private readonly IEventCatalogService _eventCatalogService;

        public SportRulesService(AppDbContext context, IHubContext<MatchLiveHub> hub, IEventCatalogService eventCatalogService)
        {
            _context = context;
            _hub = hub;
            _eventCatalogService = eventCatalogService;
        }

        public List<SportRuleDto> GetRules()
        {
            return new List<SportRuleDto>
            {
                Rule("Football","Футбол","Team", true, 2, "2:1",
                    new[] {"goal","assist","yellow_card","red_card","substitution","penalty","corner","offside","foul","injury","var_check","period_start","period_end","note"},
                    new[] {"score","winner","possession","shots","shots_on_target","corners","fouls","yellow_cards","red_cards","offsides","saves","notes"}),

                Rule("Basketball","Баскетбол","Team", false, 4, "78:72",
                    new[] {"point_1","point_2","point_3","assist","rebound","steal","block","turnover","foul","timeout","substitution","quarter_start","quarter_end","note"},
                    new[] {"score","winner","q1","q2","q3","q4","overtime","fouls","rebounds","assists","steals","blocks","turnovers","notes"}),

                Rule("Volleyball","Волейбол","Team", false, 5, "3:1",
                    new[] {"point","ace","block","attack","service_error","receive_error","rotation","timeout","substitution","set_start","set_end","note"},
                    new[] {"score","winner","sets","set1","set2","set3","set4","set5","aces","blocks","errors","notes"}),

                Rule("Tennis","Теніс","Individual", false, 5, "2:1",
                    new[] {"point","ace","double_fault","break_point","game","set","medical_timeout","warning","note"},
                    new[] {"score","winner","sets","set1","set2","set3","set4","set5","aces","double_faults","break_points","notes"}),

                Rule("Boxing","Бокс","Individual", false, 12, "30:27",
                    new[] {"round_start","round_end","knockdown","standing_count","warning","point_deduction","doctor_check","ko","tko","note"},
                    new[] {"score","winner","method","rounds","judge_cards","knockdowns","deductions","notes"}),

                Rule("Chess","Шахи","Individual", true, 1, "1:0",
                    new[] {"move","check","capture","castle","promotion","draw_offer","resign","time_pressure","illegal_move","note"},
                    new[] {"score","winner","result","moves","opening","time_white","time_black","termination","notes"}),

                Rule("Checkers","Шашки","Individual", true, 1, "1:0",
                    new[] {"move","capture","king","draw_offer","resign","time_pressure","note"},
                    new[] {"score","winner","result","moves","time_first","time_second","termination","notes"}),

                Rule("Hockey","Хокей","Team", true, 3, "4:3",
                    new[] {"goal","assist","penalty","power_play","save","substitution","period_start","period_end","note"},
                    new[] {"score","winner","period1","period2","period3","overtime","shots","penalties","saves","notes"}),

                Rule("Baseball","Бейсбол","Team", false, 9, "5:3",
                    new[] {"run","hit","home_run","strikeout","walk","error","inning_start","inning_end","note"},
                    new[] {"score","winner","innings","hits","errors","home_runs","strikeouts","walks","notes"}),

                Rule("Badminton","Бадмінтон","Individual", false, 3, "2:0",
                    new[] {"point","ace","fault","game_start","game_end","timeout","note"},
                    new[] {"score","winner","games","game1","game2","game3","notes"}),

                Rule("TableTennis","Настільний теніс","Individual", false, 7, "3:1",
                    new[] {"point","ace","fault","game_start","game_end","timeout","note"},
                    new[] {"score","winner","sets","set1","set2","set3","set4","set5","set6","set7","notes"}),

                Rule("Wrestling","Боротьба","Individual", false, 3, "8:5",
                    new[] {"takedown","escape","reversal","near_fall","warning","penalty","pin","period_start","period_end","note"},
                    new[] {"score","winner","method","periods","penalties","pin_time","notes"})
            };
        }

        public SportRuleDto GetRule(string sport)
        {
            return GetRules().FirstOrDefault(x => string.Equals(x.Sport, sport, StringComparison.OrdinalIgnoreCase))
                ?? Rule("Generic","Інший спорт","Mixed", true, 0, "1:0",
                    new[] {"score_change","warning","penalty","period_start","period_end","note"},
                    new[] {"score","winner","periods","penalties","notes"});
        }

        public async Task<SportMatchStateDto> GetMatchStateAsync(string matchType, long matchId, string? login, string? role)
        {
            var core = await GetCoreMatchAsync(matchType, matchId);
            var parsed = ParseInfo(core.AddInformation);
            var rule = GetRule(core.Sport);
            var timeline = await GetLiveEventsAsync(core.MatchType, core.MatchId);

            return new SportMatchStateDto
            {
                MatchType = core.MatchType,
                MatchId = core.MatchId,
                IdEvent = core.IdEvent,
                Sport = core.Sport,
                Status = core.Status,
                FirstParticipant = core.FirstParticipant,
                SecondParticipant = core.SecondParticipant,
                Score = parsed.GetValueOrDefault("score"),
                Winner = parsed.GetValueOrDefault("winner"),
                CurrentPeriod = ToInt(parsed.GetValueOrDefault("period")),
                Time = parsed.GetValueOrDefault("time"),
                Timeline = timeline,
                Stats = ParseStats(parsed),
                Rules = rule,
                CanEdit = await CanEditMatchAsync(core.MatchType, core.MatchId, login, role),
                ValidationMessages = ValidateFinalScore(rule.Sport, parsed.GetValueOrDefault("score") ?? "", core.FirstParticipant, core.SecondParticipant, parsed.GetValueOrDefault("winner")).Errors
            };
        }

        public async Task<SportMatchStateDto> AddLiveEventAsync(SportLiveEventSubmitDto dto)
        {
            if (!await CanEditMatchAsync(dto.MatchType, dto.MatchId, dto.Login, dto.Role))
                throw new UnauthorizedAccessException("You do not have permissions to edit this match");

            var state = await GetMatchStateAsync(dto.MatchType, dto.MatchId, dto.Login, dto.Role);
            var rule = GetRule(state.Sport);

            if (!rule.LiveEvents.Contains(dto.Type))
                throw new ArgumentException($"Подія '{dto.Type}' не дозволена для {rule.DisplayName}");

            ValidateLiveEvent(dto, rule);

            var info = ParseInfo(await GetInfoStringAsync(dto.MatchType, dto.MatchId));
            ApplyLiveScore(rule.Sport, info, dto, state);
            info["period"] = (dto.Period ?? state.CurrentPeriod).ToString();
            if (dto.Minute.HasValue) info["time"] = dto.Minute.Value.ToString();

            await InsertLiveEventAsync(dto, state);
            await SetMatchInfoAsync(dto.MatchType, dto.MatchId, ToInfoString(info), StatusMatch.Live);

            var updated = await GetMatchStateAsync(dto.MatchType, dto.MatchId, dto.Login, dto.Role);
            await BroadcastAsync(updated);
            return updated;
        }

        public async Task<SportMatchStateDto> SubmitFinalResultAsync(SportFinalResultSubmitDto dto)
        {
            if (!await CanEditMatchAsync(dto.MatchType, dto.MatchId, dto.Login, dto.Role))
                throw new UnauthorizedAccessException("You do not have permissions to edit this match");

            var state = await GetMatchStateAsync(dto.MatchType, dto.MatchId, dto.Login, dto.Role);
            var validation = ValidateFinalScore(state.Sport, dto.Score, state.FirstParticipant, state.SecondParticipant, dto.Winner);

            if (!validation.IsValid)
                throw new ArgumentException(string.Join("; ", validation.Errors));

            var info = ParseInfo(await GetInfoStringAsync(dto.MatchType, dto.MatchId));

            info["score"] = Sanitize(dto.Score);
            info["winner"] = Sanitize(string.IsNullOrWhiteSpace(dto.Winner) ? validation.Winner : dto.Winner);
            info["notes"] = Sanitize(dto.Notes);
            info["finishedAt"] = DateTime.UtcNow.ToString("O");

            foreach (var kv in dto.Stats)
                info[$"stat_{kv.Key}"] = Sanitize(kv.Value);

            await SetMatchInfoAsync(dto.MatchType, dto.MatchId, ToInfoString(info), dto.FinishMatch ? StatusMatch.Finished : StatusMatch.Live);

            var updated = await GetMatchStateAsync(dto.MatchType, dto.MatchId, dto.Login, dto.Role);

            if (dto.FinishMatch)
            {
                await TryAdvanceTournamentAsync(updated.IdEvent, dto.Login, dto.Role);
                updated = await GetMatchStateAsync(dto.MatchType, dto.MatchId, dto.Login, dto.Role);
            }

            await BroadcastAsync(updated);
            return updated;
        }

        private async Task TryAdvanceTournamentAsync(long idEvent, string? login, string? role)
        {
            try
            {
                await _eventCatalogService.GenerateNextRoundAsync(idEvent, login, role);
            }
            catch
            {
                // Not every sport/system should generate a new round immediately.
                // We keep final result saving stable and leave manual generation available.
            }
        }

        public SportScoreValidationResultDto ValidateFinalScore(string sport, string score, string firstParticipant, string secondParticipant, string? winner)
        {
            var rule = GetRule(sport);
            var result = new SportScoreValidationResultDto { IsValid = true };

            if (string.IsNullOrWhiteSpace(score))
            {
                result.IsValid = false;
                result.Errors.Add("Рахунок обов'язковий.");
                return result;
            }

            var normalized = score.Trim();

            if (sport is "Chess" or "Checkers")
            {
                if (normalized is not ("1:0" or "0:1" or "0.5:0.5" or "1/2:1/2"))
                {
                    result.IsValid = false;
                    result.Errors.Add("Для шахів/шашок дозволено 1:0, 0:1, 0.5:0.5.");
                }

                result.Winner = normalized == "1:0" ? firstParticipant : normalized == "0:1" ? secondParticipant : "";
                return result;
            }

            var parsed = ParseScore(normalized);

            if (!parsed.HasValue)
            {
                if (sport == "Boxing" && Regex.IsMatch(normalized, @"^(KO|TKO|DQ|RTD)\s*R?\d*$", RegexOptions.IgnoreCase))
                {
                    result.Winner = winner ?? "";
                    return result;
                }

                result.IsValid = false;
                result.Errors.Add($"Невірний формат рахунку. Приклад: {rule.ScorePatternHint}");
                return result;
            }

            if (!rule.AllowDraw && parsed.Value.Item1 == parsed.Value.Item2)
            {
                result.IsValid = false;
                result.Errors.Add($"{rule.DisplayName}: нічия не дозволена, потрібно вказати переможця або додатковий результат.");
            }

            if (!string.IsNullOrWhiteSpace(winner) && winner != firstParticipant && winner != secondParticipant)
            {
                result.IsValid = false;
                result.Errors.Add("Переможець має бути одним з учасників матчу.");
            }

            if (string.IsNullOrWhiteSpace(winner))
            {
                if (parsed.Value.Item1 > parsed.Value.Item2) result.Winner = firstParticipant;
                else if (parsed.Value.Item2 > parsed.Value.Item1) result.Winner = secondParticipant;
                else result.Winner = "";
            }
            else
            {
                result.Winner = winner;
            }

            return result;
        }

        private void ValidateLiveEvent(SportLiveEventSubmitDto dto, SportRuleDto rule)
        {
            if (string.IsNullOrWhiteSpace(dto.Participant))
                throw new ArgumentException("Учасник події обов'язковий.");

            if (dto.Period.HasValue && rule.MaxPeriods > 0 && dto.Period.Value > rule.MaxPeriods)
                throw new ArgumentException($"Для {rule.DisplayName} максимум періодів/раундів: {rule.MaxPeriods}.");

            if (dto.Minute.HasValue && dto.Minute.Value < 0)
                throw new ArgumentException("Хвилина не може бути від'ємною.");
        }

        private void ApplyLiveScore(string sport, Dictionary<string, string> info, SportLiveEventSubmitDto dto, SportMatchStateDto state)
        {
            var scoring = new HashSet<string> { "goal", "point", "point_1", "point_2", "point_3", "run" };
            if (!scoring.Contains(dto.Type)) return;

            var score = ParseScore(info.GetValueOrDefault("score")) ?? (0, 0);
            var delta = dto.Type == "point_2" ? 2 : dto.Type == "point_3" ? 3 : 1;

            if (dto.Participant == state.FirstParticipant || dto.Participant.Equals("first", StringComparison.OrdinalIgnoreCase))
                score.Item1 += delta;
            else
                score.Item2 += delta;

            info["score"] = $"{score.Item1}:{score.Item2}";
        }

        private async Task InsertLiveEventAsync(SportLiveEventSubmitDto dto, SportMatchStateDto state)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO "MatchLiveEvent"
                ("MatchType","MatchId","IdEvent","Sport","EventType","Participant","Player","Minute","Period","Value","Notes","CreatedBy","CreatedAt")
                VALUES (@matchType,@matchId,@idEvent,@sport,@eventType,@participant,@player,@minute,@period,@value,@notes,@createdBy,now())
            """;

            Add(cmd, "@matchType", dto.MatchType.Trim().ToLower());
            Add(cmd, "@matchId", dto.MatchId);
            Add(cmd, "@idEvent", state.IdEvent);
            Add(cmd, "@sport", state.Sport);
            Add(cmd, "@eventType", dto.Type);
            Add(cmd, "@participant", dto.Participant);
            Add(cmd, "@player", dto.Player);
            Add(cmd, "@minute", dto.Minute);
            Add(cmd, "@period", dto.Period);
            Add(cmd, "@value", dto.Value);
            Add(cmd, "@notes", dto.Notes);
            Add(cmd, "@createdBy", dto.Login);

            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<List<SportLiveEventDto>> GetLiveEventsAsync(string matchType, long matchId)
        {
            var result = new List<SportLiveEventDto>();
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT "Id","EventType","Participant","Player","Minute","Period","Value","Notes","CreatedBy","CreatedAt"
                FROM "MatchLiveEvent"
                WHERE "MatchType" = @matchType AND "MatchId" = @matchId
                ORDER BY "CreatedAt" ASC, "Id" ASC
            """;
            Add(cmd, "@matchType", matchType.Trim().ToLower());
            Add(cmd, "@matchId", matchId);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new SportLiveEventDto
                {
                    Id = reader.GetInt64(0),
                    Type = reader.GetString(1),
                    Participant = reader.GetString(2),
                    Player = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Minute = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    Period = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                    Value = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Notes = reader.IsDBNull(7) ? null : reader.GetString(7),
                    CreatedBy = reader.IsDBNull(8) ? null : reader.GetString(8),
                    CreatedAt = reader.GetDateTime(9)
                });
            }

            return result;
        }

        private async Task BroadcastAsync(SportMatchStateDto state)
        {
            await _hub.Clients.Group(MatchLiveHub.GroupName(state.MatchType, state.MatchId))
                .SendAsync("MatchUpdated", state);
        }

        private async Task<CoreMatch> GetCoreMatchAsync(string matchType, long matchId)
        {
            matchType = matchType.Trim().ToLower();

            if (matchType == "team")
            {
                var match = await _context.TeamMatches.AsNoTracking().FirstOrDefaultAsync(x => x.IdTeamMatch == matchId);
                if (match == null) throw new NotFoundException("Team match not found");
                var ev = await _context.Events.AsNoTracking().FirstAsync(e => e.IdEvent == match.IdEvent);

                return new CoreMatch("team", match.IdTeamMatch, match.IdEvent, ev.TypeSport, match.StatusMatch.ToString(), match.NameFirstTeam, match.NameSecondTeam, match.AddInformation);
            }

            if (matchType == "individual")
            {
                var match = await _context.IndividualMatches.AsNoTracking().FirstOrDefaultAsync(x => x.IdIndividualMatch == matchId);
                if (match == null) throw new NotFoundException("Individual match not found");
                var ev = await _context.Events.AsNoTracking().FirstAsync(e => e.IdEvent == match.IdEvent);

                var first = await _context.Athletes.AsNoTracking().FirstOrDefaultAsync(a => a.login == match.loginFirstAthlete);
                var second = await _context.Athletes.AsNoTracking().FirstOrDefaultAsync(a => a.login == match.loginSecondAthlete);

                return new CoreMatch(
                    "individual",
                    match.IdIndividualMatch,
                    match.IdEvent,
                    ev.TypeSport,
                    match.StatusMatch.ToString(),
                    first == null ? match.loginFirstAthlete : $"{first.FirsName} {first.LastName}",
                    second == null ? match.loginSecondAthlete : $"{second.FirsName} {second.LastName}",
                    match.AddInformation);
            }

            if (matchType == "extreme")
            {
                var match = await _context.ExtremeMatches.AsNoTracking().FirstOrDefaultAsync(x => x.IdExtremeMatches == matchId);
                if (match == null) throw new NotFoundException("Extreme match not found");
                var ev = await _context.Events.AsNoTracking().FirstAsync(e => e.IdEvent == match.IdEvent);

                return new CoreMatch("extreme", match.IdExtremeMatches, match.IdEvent, ev.TypeSport, match.StatusMatch.ToString(), "Participants", "Standard", match.AddInformation);
            }

            throw new ArgumentException("Unknown match type");
        }

        private async Task<string?> GetInfoStringAsync(string matchType, long matchId)
        {
            matchType = matchType.Trim().ToLower();

            if (matchType == "team")
                return await _context.TeamMatches.Where(m => m.IdTeamMatch == matchId).Select(m => m.AddInformation).FirstOrDefaultAsync();

            if (matchType == "individual")
                return await _context.IndividualMatches.Where(m => m.IdIndividualMatch == matchId).Select(m => m.AddInformation).FirstOrDefaultAsync();

            if (matchType == "extreme")
                return await _context.ExtremeMatches.Where(m => m.IdExtremeMatches == matchId).Select(m => m.AddInformation).FirstOrDefaultAsync();

            return null;
        }

        private async Task SetMatchInfoAsync(string matchType, long matchId, string info, StatusMatch status)
        {
            matchType = matchType.Trim().ToLower();

            if (matchType == "team")
            {
                var match = await _context.TeamMatches.FirstAsync(m => m.IdTeamMatch == matchId);
                match.AddInformation = info;
                match.StatusMatch = status;
            }
            else if (matchType == "individual")
            {
                var match = await _context.IndividualMatches.FirstAsync(m => m.IdIndividualMatch == matchId);
                match.AddInformation = info;
                match.StatusMatch = status;
            }
            else if (matchType == "extreme")
            {
                var match = await _context.ExtremeMatches.FirstAsync(m => m.IdExtremeMatches == matchId);
                match.AddInformation = info;
                match.StatusMatch = status;
            }

            await _context.SaveChangesAsync();
        }

        private async Task<bool> CanEditMatchAsync(string matchType, long matchId, string? login, string? role)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(role)) return false;

            matchType = matchType.Trim().ToLower();

            if (matchType == "team")
            {
                var match = await _context.TeamMatches.AsNoTracking().FirstOrDefaultAsync(m => m.IdTeamMatch == matchId);
                return match != null && await CanEditTeamMatchAsync(match, login, role);
            }

            if (matchType == "individual")
            {
                var match = await _context.IndividualMatches.AsNoTracking().FirstOrDefaultAsync(m => m.IdIndividualMatch == matchId);
                return match != null && await CanEditIndividualMatchAsync(match, login, role);
            }

            if (matchType == "extreme")
            {
                var match = await _context.ExtremeMatches.AsNoTracking().FirstOrDefaultAsync(m => m.IdExtremeMatches == matchId);
                return match != null && await CanEditExtremeMatchAsync(match, login, role);
            }

            return false;
        }

        private async Task<bool> CanEditTeamMatchAsync(TeamMatch m, string? login, string? role)
        {
            if (role == "Judge" && m.loginJudge == login) return true;

            if (role == "Organization")
            {
                var teams = await _context.OrganizationTeams
                    .AsNoTracking()
                    .Where(x => x.LoginOrganization == login)
                    .Select(x => x.NameComand)
                    .ToListAsync();

                return teams.Contains(m.NameFirstTeam) || teams.Contains(m.NameSecondTeam);
            }

            return false;
        }

        private Task<bool> CanEditIndividualMatchAsync(IndividualMatch m, string? login, string? role)
        {
            return Task.FromResult(role == "Judge" && m.loginJudge == login);
        }

        private Task<bool> CanEditExtremeMatchAsync(ExtremeMatch m, string? login, string? role)
        {
            return Task.FromResult(role == "Judge" && m.loginJudge == login);
        }

        private SportRuleDto Rule(string sport, string displayName, string mode, bool allowDraw, int maxPeriods, string hint, IEnumerable<string> liveEvents, IEnumerable<string> resultFields)
        {
            var eventLabels = liveEvents.ToDictionary(x => x, Humanize);
            var fieldLabels = resultFields.ToDictionary(x => x, Humanize);

            fieldLabels["score"] = "Рахунок";
            fieldLabels["winner"] = "Переможець";
            fieldLabels["notes"] = "Нотатки";
            fieldLabels["method"] = "Метод перемоги";

            return new SportRuleDto
            {
                Sport = sport,
                DisplayName = displayName,
                MatchMode = mode,
                LiveEvents = liveEvents.ToList(),
                ResultFields = resultFields.ToList(),
                ScoreExamples = new List<string> { hint },
                ScorePatternHint = hint,
                FieldLabels = fieldLabels,
                EventLabels = eventLabels,
                AllowDraw = allowDraw,
                MaxPeriods = maxPeriods
            };
        }

        private Dictionary<string, string> ParseInfo(string? value)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(value)) return result;

            foreach (var part in value.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var index = part.IndexOf('=');
                if (index <= 0) continue;

                var key = part[..index].Trim();
                var val = part[(index + 1)..].Trim();

                if (!string.IsNullOrWhiteSpace(key)) result[key] = val;
            }

            return result;
        }

        private Dictionary<string, string> ParseStats(Dictionary<string, string> parsed)
        {
            return parsed
                .Where(kv => kv.Key.StartsWith("stat_", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(kv => kv.Key.Replace("stat_", ""), kv => kv.Value);
        }

        private string ToInfoString(Dictionary<string, string> parsed)
        {
            return string.Join(";", parsed.Select(kv => $"{kv.Key}={kv.Value}"));
        }

        private (int, int)? ParseScore(string? score)
        {
            if (string.IsNullOrWhiteSpace(score)) return null;

            var match = Regex.Match(score, @"(-?\d+)\s*[:\-]\s*(-?\d+)");
            if (!match.Success) return null;

            return (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
        }

        private int ToInt(string? value) => int.TryParse(value, out var result) ? result : 0;

        private string Sanitize(string? value)
        {
            return (value ?? "").Replace(";", ",").Replace("=", ":").Trim();
        }

        private string Humanize(string value)
        {
            return string.Join(" ", value.Split('_').Select(x => char.ToUpper(x[0]) + x[1..]));
        }

        private static void Add(IDbCommand cmd, string name, object? value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(parameter);
        }

        private record CoreMatch(
            string MatchType,
            long MatchId,
            long IdEvent,
            string Sport,
            string Status,
            string FirstParticipant,
            string SecondParticipant,
            string? AddInformation
        );
    }
}
