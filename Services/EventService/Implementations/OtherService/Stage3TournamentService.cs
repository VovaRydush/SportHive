using System.ComponentModel.DataAnnotations;
using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SportHive.Exceptions;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class Stage3TournamentService : IStage3TournamentService
    {
        private readonly AppDbContext _context;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        private readonly IMongoCollection<TeamIndivGrid> _grid;
        private readonly IMongoCollection<SwissSystemPlayed> _swissPlayed;

        public Stage3TournamentService(AppDbContext context, IMongoDbService mongoDbService)
        {
            _context = context;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _grid = mongoDbService.GetCollection<TeamIndivGrid>("TeamIndivGrid");
            _swissPlayed = mongoDbService.GetCollection<SwissSystemPlayed>("SwissSystemPlayed");
        }

        public async Task<Stage3EventWorkspaceDto> CreateEventAsync(Stage3CreateEventDto dto)
        {
            ValidateCreateEvent(dto.NameEvent, dto.Participants);

            var existing = await _context.Events.FirstOrDefaultAsync(e => e.NameEvent == dto.NameEvent);
            if (existing != null)
                throw new ValidationException("Event with this name already exists");

            var entity = new Event
            {
                NameEvent = dto.NameEvent.Trim(),
                systems = dto.Systems,
                TypeSport = NormalizeSport(dto.TypeSport),
                EventPhoto = "",
                DataStart = ToUtc(dto.DataStart),
                DataEnd = dto.DataEnd.HasValue ? ToUtc(dto.DataEnd.Value) : null,
                description = dto.Description ?? ""
            };

            _context.Events.Add(entity);
            await _context.SaveChangesAsync();

            if (dto.GenerateMatches)
            {
                await GenerateInitialMatches(entity, dto.ParticipantType, dto.Participants, dto.LoginJudge);
            }

            return await GetWorkspaceAsync(entity.NameEvent);
        }

        public async Task<Stage3EventWorkspaceDto> GenerateMatchesAsync(Stage3GenerateMatchesDto dto)
        {
            var ev = await GetEventByName(dto.NameEvent);

            var alreadyHasMatches = await _context.TeamMatches.AnyAsync(m => m.IdEvent == ev.IdEvent)
                || await _context.IndividualMatches.AnyAsync(m => m.IdEvent == ev.IdEvent)
                || await _context.ExtremeMatches.AnyAsync(m => m.IdEvent == ev.IdEvent);

            if (alreadyHasMatches)
                throw new ValidationException("Matches for this event already exist");

            await GenerateInitialMatches(ev, dto.ParticipantType, dto.Participants, dto.LoginJudge);
            return await GetWorkspaceAsync(ev.NameEvent);
        }

        public async Task<Stage3EventWorkspaceDto> GetWorkspaceAsync(string eventName)
        {
            var ev = await GetEventByName(eventName);
            var matches = new List<Stage3MatchDto>();

            matches.AddRange(await _context.TeamMatches
                .AsNoTracking()
                .Where(m => m.IdEvent == ev.IdEvent)
                .Select(m => new Stage3MatchDto
                {
                    IdMatch = m.IdTeamMatch,
                    IdEvent = m.IdEvent,
                    MatchType = "team",
                    Entity1 = m.NameFirstTeam,
                    Entity2 = m.NameSecondTeam,
                    Tour = m.Tour,
                    Group = m.Group,
                    Status = m.StatusMatch.ToString(),
                    DataMatch = m.DataMatch,
                    TimeMatch = m.TimeMatch,
                    LocationName = m.LocationName,
                    LoginJudge = m.loginJudge,
                    AddInformation = m.AddInformation
                })
                .ToListAsync());

            matches.AddRange(await _context.IndividualMatches
                .AsNoTracking()
                .Where(m => m.IdEvent == ev.IdEvent)
                .Select(m => new Stage3MatchDto
                {
                    IdMatch = m.IdIndividualMatch,
                    IdEvent = m.IdEvent,
                    MatchType = "individual",
                    Entity1 = m.loginFirstAthlete,
                    Entity2 = m.loginSecondAthlete,
                    Tour = m.Tour,
                    Group = null,
                    Status = m.StatusMatch.ToString(),
                    DataMatch = m.DataMatch,
                    TimeMatch = m.TimeMatch,
                    LocationName = m.LocationName,
                    LoginJudge = m.loginJudge,
                    AddInformation = m.AddInformation
                })
                .ToListAsync());

            matches.AddRange(await _context.ExtremeMatches
                .AsNoTracking()
                .Where(m => m.IdEvent == ev.IdEvent)
                .Select(m => new Stage3MatchDto
                {
                    IdMatch = m.IdExtremeMatches,
                    IdEvent = m.IdEvent,
                    MatchType = "extreme",
                    Entity1 = "Учасники",
                    Entity2 = "Норматив/заїзд",
                    Tour = m.Tour,
                    Group = null,
                    Status = m.StatusMatch.ToString(),
                    DataMatch = m.DataMatch,
                    TimeMatch = m.TimeMatch,
                    LocationName = m.LocationName,
                    LoginJudge = m.loginJudge,
                    AddInformation = m.AddInformation
                })
                .ToListAsync());

            var mongo = await _matchEvents.Find(x => x.idEvent == ev.IdEvent).ToListAsync();
            foreach (var match in matches)
            {
                var record = mongo.FirstOrDefault(x => x.idMatch == match.IdMatch);
                if (record == null) continue;

                match.Score1 = ParseScore(record.firstTeamScore);
                match.Score2 = ParseScore(record.secondTeamScore);
                match.Winner = string.IsNullOrWhiteSpace(record.NameWinner) ? null : record.NameWinner;
                match.Played = !string.IsNullOrWhiteSpace(record.NameWinner)
                    || match.Status.Equals(StatusMatch.Finished.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            matches = matches
                .OrderBy(m => m.Tour)
                .ThenBy(m => m.Group ?? 0)
                .ThenBy(m => m.IdMatch)
                .ToList();

            var standings = BuildStandings(matches);

            return new Stage3EventWorkspaceDto
            {
                IdEvent = ev.IdEvent,
                NameEvent = ev.NameEvent,
                TypeSport = ev.TypeSport,
                System = ev.systems,
                DataStart = ev.DataStart,
                DataEnd = ev.DataEnd,
                Description = ev.description,
                Matches = matches,
                Standings = standings,
                Summary = new Stage3EventSummaryDto
                {
                    ParticipantsCount = standings.Count,
                    MatchesCount = matches.Count,
                    FinishedMatches = matches.Count(m => m.Status == StatusMatch.Finished.ToString()),
                    LiveMatches = matches.Count(m => m.Status == StatusMatch.Live.ToString()),
                    UpcomingMatches = matches.Count(m => m.Status == StatusMatch.Upcoming.ToString()),
                    Winner = standings.OrderByDescending(s => s.Points).ThenByDescending(s => s.ScoreDiff).FirstOrDefault()?.Name
                }
            };
        }

        public async Task<Stage3MatchDto> GetMatchAsync(string matchType, long idMatch)
        {
            Stage3MatchDto? match = null;
            var type = NormalizeParticipantType(matchType);

            if (type == "team")
            {
                match = await _context.TeamMatches.AsNoTracking()
                    .Where(m => m.IdTeamMatch == idMatch)
                    .Select(m => new Stage3MatchDto
                    {
                        IdMatch = m.IdTeamMatch,
                        IdEvent = m.IdEvent,
                        MatchType = "team",
                        Entity1 = m.NameFirstTeam,
                        Entity2 = m.NameSecondTeam,
                        Tour = m.Tour,
                        Group = m.Group,
                        Status = m.StatusMatch.ToString(),
                        DataMatch = m.DataMatch,
                        TimeMatch = m.TimeMatch,
                        LocationName = m.LocationName,
                        LoginJudge = m.loginJudge,
                        AddInformation = m.AddInformation
                    }).FirstOrDefaultAsync();
            }
            else if (type == "individual")
            {
                match = await _context.IndividualMatches.AsNoTracking()
                    .Where(m => m.IdIndividualMatch == idMatch)
                    .Select(m => new Stage3MatchDto
                    {
                        IdMatch = m.IdIndividualMatch,
                        IdEvent = m.IdEvent,
                        MatchType = "individual",
                        Entity1 = m.loginFirstAthlete,
                        Entity2 = m.loginSecondAthlete,
                        Tour = m.Tour,
                        Status = m.StatusMatch.ToString(),
                        DataMatch = m.DataMatch,
                        TimeMatch = m.TimeMatch,
                        LocationName = m.LocationName,
                        LoginJudge = m.loginJudge,
                        AddInformation = m.AddInformation
                    }).FirstOrDefaultAsync();
            }
            else
            {
                match = await _context.ExtremeMatches.AsNoTracking()
                    .Where(m => m.IdExtremeMatches == idMatch)
                    .Select(m => new Stage3MatchDto
                    {
                        IdMatch = m.IdExtremeMatches,
                        IdEvent = m.IdEvent,
                        MatchType = "extreme",
                        Entity1 = "Учасники",
                        Entity2 = "Норматив/заїзд",
                        Tour = m.Tour,
                        Status = m.StatusMatch.ToString(),
                        DataMatch = m.DataMatch,
                        TimeMatch = m.TimeMatch,
                        LocationName = m.LocationName,
                        LoginJudge = m.loginJudge,
                        AddInformation = m.AddInformation
                    }).FirstOrDefaultAsync();
            }

            if (match == null) throw new NotFoundException("Match not found");

            var record = await _matchEvents.Find(x => x.idMatch == idMatch).FirstOrDefaultAsync();
            if (record != null)
            {
                match.Score1 = ParseScore(record.firstTeamScore);
                match.Score2 = ParseScore(record.secondTeamScore);
                match.Winner = record.NameWinner;
                match.Played = !string.IsNullOrWhiteSpace(record.NameWinner);
            }

            return match;
        }

        public async Task<Stage3EventWorkspaceDto> SetResultAsync(Stage3SetMatchResultDto dto)
        {
            var match = await GetMatchAsync(dto.MatchType, dto.IdMatch);
            var ev = await _context.Events.FirstAsync(e => e.IdEvent == match.IdEvent);
            var winner = ResolveWinner(dto, match);
            var loser = ResolveLoser(winner, match);

            await UpsertMongoResult(match, dto, winner, loser);
            await UpdateSqlMatchStatus(match.MatchType, match.IdMatch, dto.CloseMatch ? StatusMatch.Finished : StatusMatch.Live, dto.Comment);

            if (dto.CloseMatch)
            {
                await TryGenerateNextRound(ev, match.MatchType, match.Tour);
            }

            return await GetWorkspaceAsync(ev.NameEvent);
        }

        public async Task<Stage3MatchDto> ChangeStatusAsync(Stage3ChangeMatchStatusDto dto)
        {
            await UpdateSqlMatchStatus(dto.MatchType, dto.IdMatch, dto.Status, null);
            return await GetMatchAsync(dto.MatchType, dto.IdMatch);
        }

        private async Task GenerateInitialMatches(Event ev, string participantType, List<string> participants, string? loginJudge)
        {
            var type = NormalizeParticipantType(participantType);
            participants = participants
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (participants.Count < 1) throw new ValidationException("Participants list is empty");

            await ValidateParticipants(type, participants);

            if (ev.systems == SelectionSystems.QualificationByStandards)
            {
                await CreateQualificationMatches(ev, type, participants, loginJudge);
                return;
            }

            var pairings = ev.systems switch
            {
                SelectionSystems.RoundRobin => BuildRoundRobin(participants),
                SelectionSystems.GroupStage => BuildGroupStage(participants),
                SelectionSystems.SwissSystem => BuildSwissRound(participants, 1, new HashSet<string>()),
                SelectionSystems.DoubleElimination => BuildSingleEliminationFirstRound(participants),
                SelectionSystems.PlayOff => BuildSingleEliminationFirstRound(participants),
                SelectionSystems.OlympicSystem => BuildSingleEliminationFirstRound(participants),
                SelectionSystems.KnockoutSystem => BuildSingleEliminationFirstRound(participants),
                SelectionSystems.Final => BuildSingleEliminationFirstRound(participants),
                SelectionSystems.MixedSystem => BuildGroupStage(participants),
                _ => BuildSingleEliminationFirstRound(participants)
            };

            foreach (var pairing in pairings)
            {
                await CreateMatch(ev.IdEvent, type, pairing.Entity1, pairing.Entity2, pairing.Tour, pairing.Group, loginJudge,
                    $"{ev.systems}; auto generated");
            }
        }

        private async Task CreateQualificationMatches(Event ev, string type, List<string> participants, string? loginJudge)
        {
            if (type == "team")
            {
                foreach (var participant in participants)
                {
                    var id = await CreateExtremeMatch(ev.IdEvent, 1, loginJudge, $"Qualification: {participant}");
                    await _context.EMatchesTeam.AddAsync(new EMatchesTeam { IdExtremeMatches = id, NameTeam = participant });
                    await SaveGrid(ev.IdEvent, id, participant, "STANDARD", 1, null, false);
                    await SaveMongoMatch(ev.IdEvent, id, participant, "STANDARD");
                }
            }
            else
            {
                foreach (var participant in participants)
                {
                    var id = await CreateExtremeMatch(ev.IdEvent, 1, loginJudge, $"Qualification: {participant}");
                    await _context.ExtremeMatchesAthetes.AddAsync(new EMatchesAthlete { IdExtremeMatches = id, loginAthlete = participant });
                    await SaveGrid(ev.IdEvent, id, participant, "STANDARD", 1, null, false);
                    await SaveMongoMatch(ev.IdEvent, id, participant, "STANDARD");
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task CreateMatch(
    long idEvent,
    string type,
    string entity1,
    string entity2,
    int tour,
    int? group,
    string? loginJudge,
    string? addInfo)
{
    var safeGroup = group ?? 1;
    var safeAddInfo = addInfo ?? "";
    var safeJudge = string.IsNullOrWhiteSpace(loginJudge) ? null : loginJudge;

    // Do NOT write "Default" here.
    // LocationName has FK to "Location"("LocationName").
    // If "Default" is not present in Location table, PostgreSQL throws 23503.
    string? safeLocation = null;

    var safeStatus = (StatusMatch)0;

    if (string.Equals(type, "team", StringComparison.OrdinalIgnoreCase))
    {
        var match = new TeamMatch
        {
            IdEvent = idEvent,
            NameFirstTeam = entity1,
            NameSecondTeam = entity2,
            Tour = tour,
            Group = safeGroup,
            AddInformation = safeAddInfo,
            StatusMatch = safeStatus,
            LocationName = safeLocation,
            loginJudge = safeJudge,
            DataMatch = DateTime.UtcNow,
            TimeMatch = TimeSpan.Zero
        };

        _context.TeamMatches.Add(match);
        await _context.SaveChangesAsync();
        return;
    }

    if (string.Equals(type, "individual", StringComparison.OrdinalIgnoreCase))
    {
        var match = new IndividualMatch
        {
            IdEvent = idEvent,
            loginFirstAthlete = entity1,
            loginSecondAthlete = entity2,
            Tour = tour,
            Group = safeGroup,
            AddInformation = safeAddInfo,
            StatusMatch = safeStatus,
            LocationName = safeLocation,
            loginJudge = safeJudge,
            DataMatch = DateTime.UtcNow,
            TimeMatch = TimeSpan.Zero
        };

        _context.IndividualMatches.Add(match);
        await _context.SaveChangesAsync();
        return;
    }

    if (string.Equals(type, "extreme", StringComparison.OrdinalIgnoreCase))
    {
        var match = new ExtremeMatch
        {
            IdEvent = idEvent,
            Tour = tour,
            Group = safeGroup,
            AddInformation = safeAddInfo,
            StatusMatch = safeStatus,
            LocationName = safeLocation,
            loginJudge = safeJudge,
            DataMatch = DateTime.UtcNow,
            TimeMatch = TimeSpan.Zero
        };

        _context.ExtremeMatches.Add(match);
        await _context.SaveChangesAsync();

        /*
          If your previous CreateMatch added rows to EMatchesAthlete / EMatchesTeam
          after creating ExtremeMatch, keep that old participant insert code here.
          The FK fix is LocationName = null.
        */

        return;
    }

    throw new InvalidOperationException($"Unknown match type: {type}");
}


        private async Task<long> CreateExtremeMatch(long idEvent, int tour, string? loginJudge, string addInfo)
        {
            var match = new ExtremeMatch
            {
                IdEvent = idEvent,
                Tour = tour,
                StatusMatch = StatusMatch.Upcoming,
                loginJudge = loginJudge,
                AddInformation = addInfo
            };
            _context.ExtremeMatches.Add(match);
            await _context.SaveChangesAsync();
            return match.IdExtremeMatches;
        }

        private async Task TryGenerateNextRound(Event ev, string matchType, int finishedTour)
        {
            if (ev.systems is SelectionSystems.RoundRobin or SelectionSystems.GroupStage or SelectionSystems.QualificationByStandards)
                return;

            var workspace = await GetWorkspaceAsync(ev.NameEvent);
            var currentTourMatches = workspace.Matches
                .Where(m => m.MatchType == matchType && m.Tour == finishedTour)
                .ToList();

            if (!currentTourMatches.Any() || currentTourMatches.Any(m => m.Status != StatusMatch.Finished.ToString()))
                return;

            var nextTour = finishedTour + 1;
            var nextAlreadyExists = workspace.Matches.Any(m => m.MatchType == matchType && m.Tour == nextTour);
            if (nextAlreadyExists) return;

            var winners = currentTourMatches
                .Where(m => !string.IsNullOrWhiteSpace(m.Winner))
                .Select(m => m.Winner!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (winners.Count <= 1) return;

            if (ev.systems == SelectionSystems.SwissSystem)
            {
                var played = workspace.Matches.Select(m => PairKey(m.Entity1, m.Entity2)).ToHashSet();
                var ordered = workspace.Standings.OrderByDescending(s => s.Points).ThenByDescending(s => s.ScoreDiff).Select(s => s.Name).ToList();
                var swissPairs = BuildSwissRound(ordered, nextTour, played);
                foreach (var pair in swissPairs)
                    await CreateMatch(ev.IdEvent, matchType, pair.Entity1, pair.Entity2, nextTour, null, null, $"Swiss round {nextTour}");
                return;
            }

            var pairs = PairSequential(winners, nextTour, null);
            foreach (var pair in pairs)
                await CreateMatch(ev.IdEvent, matchType, pair.Entity1, pair.Entity2, nextTour, null, null, $"Next round {nextTour}");
        }

        private async Task UpdateSqlMatchStatus(string matchType, long idMatch, StatusMatch status, string? comment)
        {
            var type = NormalizeParticipantType(matchType);
            if (type == "team")
            {
                var match = await _context.TeamMatches.FirstOrDefaultAsync(m => m.IdTeamMatch == idMatch)
                    ?? throw new NotFoundException("Team match not found");
                match.StatusMatch = status;
                if (!string.IsNullOrWhiteSpace(comment)) match.AddInformation = comment;
            }
            else if (type == "individual")
            {
                var match = await _context.IndividualMatches.FirstOrDefaultAsync(m => m.IdIndividualMatch == idMatch)
                    ?? throw new NotFoundException("Individual match not found");
                match.StatusMatch = status;
                if (!string.IsNullOrWhiteSpace(comment)) match.AddInformation = comment;
            }
            else
            {
                var match = await _context.ExtremeMatches.FirstOrDefaultAsync(m => m.IdExtremeMatches == idMatch)
                    ?? throw new NotFoundException("Extreme match not found");
                match.StatusMatch = status;
                if (!string.IsNullOrWhiteSpace(comment)) match.AddInformation = comment;
            }
            await _context.SaveChangesAsync();
        }

        private async Task UpsertMongoResult(Stage3MatchDto match, Stage3SetMatchResultDto dto, string winner, string loser)
        {
            var filter = Builders<MatchEvents>.Filter.Eq(x => x.idMatch, match.IdMatch);
            var update = Builders<MatchEvents>.Update
                .Set(x => x.idEvent, match.IdEvent)
                .Set(x => x.idMatch, match.IdMatch)
                .Set(x => x.composition, new List<string> { match.Entity1, match.Entity2 })
                .Set(x => x.firstTeamScore, dto.ScoreEntity1.ToString())
                .Set(x => x.secondTeamScore, dto.ScoreEntity2.ToString())
                .Set(x => x.NameWinner, winner)
                .Set(x => x.NameLosser, loser)
                .Set(x => x.Draws, dto.IsDraw ? new List<string> { match.Entity1, match.Entity2 } : new List<string>());

            await _matchEvents.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });

            var gridFilter = Builders<TeamIndivGrid>.Filter.And(
                Builders<TeamIndivGrid>.Filter.Eq(x => x.idEvent, match.IdEvent),
                Builders<TeamIndivGrid>.Filter.Eq(x => x.idMatch, match.IdMatch));

            var gridUpdate = Builders<TeamIndivGrid>.Update
                .Set(x => x.totalScoreEntity1, dto.ScoreEntity1)
                .Set(x => x.totalScoreEntity2, dto.ScoreEntity2)
                .Set(x => x.played, dto.CloseMatch)
                .Set(x => x.falloutLoser, !dto.IsDraw && dto.CloseMatch);

            await _grid.UpdateOneAsync(gridFilter, gridUpdate, new UpdateOptions { IsUpsert = false });
        }

        private async Task SaveGrid(long idEvent, long idMatch, string entity1, string entity2, int tour, int? group, bool played)
        {
            await _grid.InsertOneAsync(new TeamIndivGrid
            {
                idEvent = idEvent,
                idMatch = idMatch,
                tour = tour,
                NameFirstEntity = entity1,
                NameSecondEntity = entity2,
                totalScoreEntity1 = 0,
                totalScoreEntity2 = 0,
                falloutLoser = null,
                played = played
            });
        }

        private async Task SaveMongoMatch(long idEvent, long idMatch, string entity1, string entity2)
        {
            await _matchEvents.InsertOneAsync(new MatchEvents
            {
                idEvent = idEvent,
                idMatch = idMatch,
                composition = new List<string> { entity1, entity2 },
                firstTeamScore = "0",
                secondTeamScore = "0",
                NameWinner = "",
                NameLosser = "",
                Draws = new List<string>()
            });
        }

        private async Task ValidateParticipants(string type, List<string> participants)
        {
            if (type == "team")
            {
                var existing = await _context.Teams
                    .Where(t => participants.Contains(t.TeamName))
                    .Select(t => t.TeamName)
                    .ToListAsync();
                var missing = participants.Except(existing).ToList();
                if (missing.Any()) throw new ValidationException("Teams not found: " + string.Join(", ", missing));
            }
            else
            {
                var existing = await _context.Athletes
                    .Where(a => participants.Contains(a.login))
                    .Select(a => a.login)
                    .ToListAsync();
                var missing = participants.Except(existing).ToList();
                if (missing.Any()) throw new ValidationException("Athletes not found: " + string.Join(", ", missing));
            }
        }

        private async Task<Event> GetEventByName(string eventName)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.NameEvent == eventName)
                ?? throw new NotFoundException("Event not found");
        }

        private static List<Pairing> BuildRoundRobin(List<string> participants)
        {
            var players = participants.ToList();
            if (players.Count % 2 == 1) players.Add("BYE");
            var n = players.Count;
            var rounds = n - 1;
            var half = n / 2;
            var result = new List<Pairing>();

            for (var round = 1; round <= rounds; round++)
            {
                for (var i = 0; i < half; i++)
                {
                    var a = players[i];
                    var b = players[n - 1 - i];
                    if (a != "BYE" && b != "BYE")
                        result.Add(new Pairing(a, b, round, null));
                }
                var fixedOne = players[0];
                var rotated = players.Skip(1).ToList();
                rotated.Insert(0, rotated.Last());
                rotated.RemoveAt(rotated.Count - 1);
                players = new List<string> { fixedOne };
                players.AddRange(rotated);
            }
            return result;
        }

        private static List<Pairing> BuildGroupStage(List<string> participants)
        {
            var groupSize = participants.Count <= 8 ? 4 : 5;
            var groupCount = (int)Math.Ceiling(participants.Count / (double)groupSize);
            var groups = participants
                .Select((p, i) => new { p, group = i % groupCount + 1 })
                .GroupBy(x => x.group)
                .ToList();
            var result = new List<Pairing>();
            foreach (var group in groups)
            {
                var pairs = BuildRoundRobin(group.Select(x => x.p).ToList());
                result.AddRange(pairs.Select(p => p with { Group = group.Key }));
            }
            return result;
        }

        private static List<Pairing> BuildSingleEliminationFirstRound(List<string> participants)
        {
            var size = 1;
            while (size < participants.Count) size *= 2;
            var seeded = participants.ToList();
            while (seeded.Count < size) seeded.Add("BYE");
            var pairs = new List<Pairing>();
            for (var i = 0; i < size / 2; i++)
            {
                var a = seeded[i];
                var b = seeded[size - 1 - i];
                if (a != "BYE" && b != "BYE") pairs.Add(new Pairing(a, b, 1, null));
            }
            return pairs;
        }

        private static List<Pairing> BuildSwissRound(List<string> participants, int tour, HashSet<string> playedPairs)
        {
            var available = participants.ToList();
            var pairs = new List<Pairing>();
            while (available.Count > 1)
            {
                var first = available[0];
                available.RemoveAt(0);
                var index = available.FindIndex(x => !playedPairs.Contains(PairKey(first, x)));
                if (index < 0) index = 0;
                var second = available[index];
                available.RemoveAt(index);
                pairs.Add(new Pairing(first, second, tour, null));
            }
            return pairs;
        }

        private static List<Pairing> PairSequential(List<string> items, int tour, int? group)
        {
            var result = new List<Pairing>();
            for (var i = 0; i + 1 < items.Count; i += 2)
                result.Add(new Pairing(items[i], items[i + 1], tour, group));
            return result;
        }

        private static List<Stage3StandingDto> BuildStandings(List<Stage3MatchDto> matches)
        {
            var table = new Dictionary<string, Stage3StandingDto>(StringComparer.OrdinalIgnoreCase);
            foreach (var match in matches)
            {
                Ensure(table, match.Entity1);
                Ensure(table, match.Entity2);
                if (match.Status != StatusMatch.Finished.ToString()) continue;
                table[match.Entity1].Played++;
                table[match.Entity2].Played++;
                table[match.Entity1].ScoreFor += match.Score1;
                table[match.Entity1].ScoreAgainst += match.Score2;
                table[match.Entity2].ScoreFor += match.Score2;
                table[match.Entity2].ScoreAgainst += match.Score1;
                if (match.Score1 == match.Score2 || string.IsNullOrWhiteSpace(match.Winner))
                {
                    table[match.Entity1].Draws++;
                    table[match.Entity2].Draws++;
                    table[match.Entity1].Points += 1;
                    table[match.Entity2].Points += 1;
                }
                else if (match.Winner == match.Entity1)
                {
                    table[match.Entity1].Wins++;
                    table[match.Entity2].Losses++;
                    table[match.Entity1].Points += 3;
                }
                else
                {
                    table[match.Entity2].Wins++;
                    table[match.Entity1].Losses++;
                    table[match.Entity2].Points += 3;
                }
            }
            return table.Values
                .Where(x => x.Name != "STANDARD")
                .OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.ScoreDiff)
                .ThenByDescending(x => x.ScoreFor)
                .ToList();
        }

        private static void Ensure(Dictionary<string, Stage3StandingDto> table, string name)
        {
            if (!table.ContainsKey(name)) table[name] = new Stage3StandingDto { Name = name };
        }

        private static string ResolveWinner(Stage3SetMatchResultDto dto, Stage3MatchDto match)
        {
            if (dto.IsDraw) return "";
            if (!string.IsNullOrWhiteSpace(dto.Winner)) return dto.Winner.Trim();
            return dto.ScoreEntity1 >= dto.ScoreEntity2 ? match.Entity1 : match.Entity2;
        }

        private static string ResolveLoser(string winner, Stage3MatchDto match)
        {
            if (string.IsNullOrWhiteSpace(winner)) return "";
            return winner == match.Entity1 ? match.Entity2 : match.Entity1;
        }

        private static int ParseScore(string? score) => int.TryParse(score, out var value) ? value : 0;
        private static DateTime ToUtc(DateTime date) => date.Kind == DateTimeKind.Utc ? date : DateTime.SpecifyKind(date, DateTimeKind.Local).ToUniversalTime();
        private static string NormalizeParticipantType(string value) => value.Trim().ToLower() switch
        {
            "team" or "teams" or "команда" or "команди" => "team",
            "extreme" or "qualification" or "standard" => "extreme",
            _ => "individual"
        };
        private static string NormalizeSport(string sport) => sport.Trim();
        private static string PairKey(string a, string b) => string.Compare(a, b, StringComparison.OrdinalIgnoreCase) <= 0 ? $"{a}|{b}" : $"{b}|{a}";
        private static void ValidateCreateEvent(string name, List<string> participants)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Event name is required");
            if (participants == null || participants.Count == 0) throw new ValidationException("Participants are required");
        }

        private record Pairing(string Entity1, string Entity2, int Tour, int? Group);
    }
}
