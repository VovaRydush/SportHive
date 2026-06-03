using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Exceptions;
using SportHive.Services.Interfaces;
using System.Text.RegularExpressions;

namespace SportHive.Implementations
{
    public class EventCatalogService : IEventCatalogService
    {
        private readonly AppDbContext _context;

        public EventCatalogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventCatalogItemDto>> GetCatalogAsync(EventCatalogQueryDto query)
        {
            query.Search = query.Search?.Trim();
            query.TypeSport = query.TypeSport?.Trim();
            query.System = query.System?.Trim();
            query.Status = query.Status?.Trim();

            var events = await _context.Events
                .AsNoTracking()
                .OrderByDescending(e => e.DataStart)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower();
                events = events
                    .Where(e =>
                        e.NameEvent.ToLower().Contains(search) ||
                        e.TypeSport.ToLower().Contains(search) ||
                        e.description.ToLower().Contains(search) ||
                        e.systems.ToString().ToLower().Contains(search))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(query.TypeSport) && query.TypeSport != "all")
                events = events.Where(e => string.Equals(e.TypeSport, query.TypeSport, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(query.System) && query.System != "all")
                events = events.Where(e => string.Equals(e.systems.ToString(), query.System, StringComparison.OrdinalIgnoreCase)).ToList();

            var result = new List<EventCatalogItemDto>();

            foreach (var ev in events)
            {
                var item = await BuildEventDtoAsync(ev, query.Login, query.Role);

                if (!string.IsNullOrWhiteSpace(query.Status) && query.Status != "all")
                {
                    if (!string.Equals(item.Status, query.Status, StringComparison.OrdinalIgnoreCase))
                        continue;
                }

                result.Add(item);
            }

            return result;
        }

        public async Task<EventCatalogItemDto> GetEventAsync(long idEvent, string? login, string? role)
        {
            var ev = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.IdEvent == idEvent);

            if (ev == null)
                throw new NotFoundException("Event not found");

            return await BuildEventDtoAsync(ev, login, role);
        }

        public async Task<EventMatchCatalogDto> SubmitResultAsync(MatchResultSubmitDto dto)
        {
            if (dto == null) throw new ArgumentException("Empty result body");
            if (string.IsNullOrWhiteSpace(dto.Score)) throw new ArgumentException("Score is required");

            dto.MatchType = dto.MatchType.Trim().ToLower();

            var canEdit = await CanEditMatchAsync(dto.MatchType, dto.MatchId, dto.Login, dto.Role);
            if (!canEdit)
                throw new UnauthorizedAccessException("You do not have permissions to edit this match");

            var resultText = BuildResultText(dto);

            long eventId;

            if (dto.MatchType == "team")
            {
                var match = await _context.TeamMatches.FirstOrDefaultAsync(m => m.IdTeamMatch == dto.MatchId);
                if (match == null) throw new NotFoundException("Team match not found");

                match.AddInformation = MergeInfo(match.AddInformation, resultText);
                match.StatusMatch = dto.FinishMatch ? StatusMatch.Finished : StatusMatch.Live;
                eventId = match.IdEvent;
                await _context.SaveChangesAsync();
            }
            else if (dto.MatchType == "individual")
            {
                var match = await _context.IndividualMatches.FirstOrDefaultAsync(m => m.IdIndividualMatch == dto.MatchId);
                if (match == null) throw new NotFoundException("Individual match not found");

                match.AddInformation = MergeInfo(match.AddInformation, resultText);
                match.StatusMatch = dto.FinishMatch ? StatusMatch.Finished : StatusMatch.Live;
                eventId = match.IdEvent;
                await _context.SaveChangesAsync();
            }
            else if (dto.MatchType == "extreme")
            {
                var match = await _context.ExtremeMatches.FirstOrDefaultAsync(m => m.IdExtremeMatches == dto.MatchId);
                if (match == null) throw new NotFoundException("Extreme match not found");

                match.AddInformation = MergeInfo(match.AddInformation, resultText);
                match.StatusMatch = dto.FinishMatch ? StatusMatch.Finished : StatusMatch.Live;
                eventId = match.IdEvent;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Unknown match type");
            }

            await AutoAdvanceEventAsync(eventId);

            if (dto.MatchType == "team")
                return (await BuildTeamMatchesAsync(eventId, dto.Login, dto.Role)).First(m => m.MatchId == dto.MatchId);
            if (dto.MatchType == "individual")
                return (await BuildIndividualMatchesAsync(eventId, dto.Login, dto.Role)).First(m => m.MatchId == dto.MatchId);

            return (await BuildExtremeMatchesAsync(eventId, dto.Login, dto.Role)).First(m => m.MatchId == dto.MatchId);
        }

        public async Task<EventCatalogItemDto> GenerateNextRoundAsync(long idEvent, string? login, string? role)
        {
            if (!await CanManageEventAsync(idEvent, login, role))
                throw new UnauthorizedAccessException("You do not have permissions to generate next round");

            await AutoAdvanceEventAsync(idEvent, force: true);
            return await GetEventAsync(idEvent, login, role);
        }

        public async Task<EventCatalogItemDto> RebuildEventAsync(long idEvent, string? login, string? role)
        {
            if (!await CanManageEventAsync(idEvent, login, role))
                throw new UnauthorizedAccessException("You do not have permissions to rebuild event");

            await AutoAdvanceEventAsync(idEvent, force: true);
            return await GetEventAsync(idEvent, login, role);
        }

        private async Task<EventCatalogItemDto> BuildEventDtoAsync(Event ev, string? login, string? role)
        {
            var matches = new List<EventMatchCatalogDto>();
            matches.AddRange(await BuildTeamMatchesAsync(ev.IdEvent, login, role));
            matches.AddRange(await BuildIndividualMatchesAsync(ev.IdEvent, login, role));
            matches.AddRange(await BuildExtremeMatchesAsync(ev.IdEvent, login, role));

            var status = ResolveEventStatus(ev, matches);
            var canManage = matches.Any(m => m.CanEdit) || await IsRelatedOrganizationAsync(ev.IdEvent, login, role);

            var standings = BuildStandings(matches);
            var bracket = matches
                .GroupBy(m => new { m.Tour, m.Group, Bracket = m.BracketCode ?? "MAIN" })
                .OrderBy(g => g.Key.Tour)
                .ThenBy(g => g.Key.Group ?? 0)
                .Select(g => new BracketRoundDto
                {
                    Tour = g.Key.Tour,
                    Group = g.Key.Group,
                    BracketCode = g.Key.Bracket,
                    Matches = g.OrderBy(m => m.MatchId).ToList()
                })
                .ToList();

            return new EventCatalogItemDto
            {
                IdEvent = ev.IdEvent,
                NameEvent = ev.NameEvent,
                System = ev.systems.ToString(),
                TypeSport = ev.TypeSport,
                DataStart = ev.DataStart,
                DataEnd = ev.DataEnd,
                Description = ev.description,
                EventPhoto = ev.EventPhoto,
                Status = status,
                TotalMatches = matches.Count,
                LiveMatches = matches.Count(m => m.Status == "Live"),
                FinishedMatches = matches.Count(m => m.Status == "Finished"),
                UpcomingMatches = matches.Count(m => m.Status == "Upcoming"),
                CanManageEvent = canManage,
                AccessLevel = canManage ? "Manage" : "View",
                Matches = matches.OrderBy(m => m.Tour).ThenBy(m => m.Group ?? 0).ThenBy(m => m.MatchId).ToList(),
                Standings = standings,
                Bracket = bracket
            };
        }

        private async Task<List<EventMatchCatalogDto>> BuildTeamMatchesAsync(long idEvent, string? login, string? role)
        {
            var matches = await _context.TeamMatches.AsNoTracking().Where(m => m.IdEvent == idEvent).ToListAsync();
            var result = new List<EventMatchCatalogDto>();

            foreach (var m in matches)
            {
                var canEdit = await CanEditTeamMatchAsync(m, login, role);
                var parsed = ParseInfo(m.AddInformation);

                result.Add(new EventMatchCatalogDto
                {
                    MatchType = "team",
                    MatchId = m.IdTeamMatch,
                    IdEvent = m.IdEvent,
                    FirstParticipant = m.NameFirstTeam,
                    SecondParticipant = m.NameSecondTeam,
                    DataMatch = m.DataMatch,
                    TimeMatch = m.TimeMatch,
                    Tour = m.Tour,
                    Group = m.Group,
                    LocationName = m.LocationName,
                    Status = m.StatusMatch.ToString(),
                    LoginJudge = m.loginJudge,
                    AddInformation = m.AddInformation,
                    Score = parsed.GetValueOrDefault("score"),
                    Winner = parsed.GetValueOrDefault("winner"),
                    BracketCode = parsed.GetValueOrDefault("bracket") ?? "MAIN",
                    CanEdit = canEdit,
                    AccessReason = canEdit ? ResolveEditReason(m.loginJudge, login, role) : "ViewOnly"
                });
            }

            return result;
        }

        private async Task<List<EventMatchCatalogDto>> BuildIndividualMatchesAsync(long idEvent, string? login, string? role)
        {
            var matches = await _context.IndividualMatches.AsNoTracking().Where(m => m.IdEvent == idEvent).ToListAsync();
            var athletes = await _context.Athletes.AsNoTracking().ToListAsync();
            var byLogin = athletes.ToDictionary(a => a.login, a => $"{a.FirsName} {a.LastName}".Trim());

            var result = new List<EventMatchCatalogDto>();

            foreach (var m in matches)
            {
                var canEdit = await CanEditIndividualMatchAsync(m, login, role);
                var parsed = ParseInfo(m.AddInformation);

                result.Add(new EventMatchCatalogDto
                {
                    MatchType = "individual",
                    MatchId = m.IdIndividualMatch,
                    IdEvent = m.IdEvent,
                    FirstParticipant = byLogin.TryGetValue(m.loginFirstAthlete, out var first) ? first : m.loginFirstAthlete,
                    SecondParticipant = byLogin.TryGetValue(m.loginSecondAthlete, out var second) ? second : m.loginSecondAthlete,
                    FirstLogin = m.loginFirstAthlete,
                    SecondLogin = m.loginSecondAthlete,
                    DataMatch = m.DataMatch,
                    TimeMatch = m.TimeMatch,
                    Tour = m.Tour,
                    Group = m.Group,
                    LocationName = m.LocationName,
                    Status = m.StatusMatch.ToString(),
                    LoginJudge = m.loginJudge,
                    AddInformation = m.AddInformation,
                    Score = parsed.GetValueOrDefault("score"),
                    Winner = parsed.GetValueOrDefault("winner"),
                    BracketCode = parsed.GetValueOrDefault("bracket") ?? "MAIN",
                    CanEdit = canEdit,
                    AccessReason = canEdit ? ResolveEditReason(m.loginJudge, login, role) : "ViewOnly"
                });
            }

            return result;
        }

        private async Task<List<EventMatchCatalogDto>> BuildExtremeMatchesAsync(long idEvent, string? login, string? role)
        {
            var matches = await _context.ExtremeMatches.AsNoTracking().Where(m => m.IdEvent == idEvent).ToListAsync();
            var athletes = await _context.Athletes.AsNoTracking().ToListAsync();
            var athleteNames = athletes.ToDictionary(a => a.login, a => $"{a.FirsName} {a.LastName}".Trim());

            var result = new List<EventMatchCatalogDto>();

            foreach (var m in matches)
            {
                var athleteLogins = await _context.ExtremeMatchesAthetes
                    .AsNoTracking()
                    .Where(x => x.IdExtremeMatches == m.IdExtremeMatches)
                    .Select(x => x.loginAthlete)
                    .ToListAsync();

                var teams = await _context.EMatchesTeam
                    .AsNoTracking()
                    .Where(x => x.IdExtremeMatches == m.IdExtremeMatches)
                    .Select(x => x.NameTeam)
                    .ToListAsync();

                var names = athleteLogins.Select(x => athleteNames.TryGetValue(x, out var n) ? n : x).Concat(teams).ToList();

                var canEdit = await CanEditExtremeMatchAsync(m, login, role);
                var parsed = ParseInfo(m.AddInformation);

                result.Add(new EventMatchCatalogDto
                {
                    MatchType = "extreme",
                    MatchId = m.IdExtremeMatches,
                    IdEvent = m.IdEvent,
                    FirstParticipant = names.Count > 0 ? string.Join(", ", names) : "Participants",
                    SecondParticipant = "Standard / Track",
                    DataMatch = m.DataMatch,
                    TimeMatch = m.TimeMatch,
                    Tour = m.Tour,
                    Group = m.Group,
                    LocationName = m.LocationName,
                    Status = m.StatusMatch.ToString(),
                    LoginJudge = m.loginJudge,
                    AddInformation = m.AddInformation,
                    Score = parsed.GetValueOrDefault("score"),
                    Winner = parsed.GetValueOrDefault("winner"),
                    BracketCode = parsed.GetValueOrDefault("bracket") ?? "MAIN",
                    CanEdit = canEdit,
                    AccessReason = canEdit ? ResolveEditReason(m.loginJudge, login, role) : "ViewOnly"
                });
            }

            return result;
        }

        private async Task AutoAdvanceEventAsync(long idEvent, bool force = false)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.IdEvent == idEvent);
            if (ev == null) return;

            var hasTeams = await _context.TeamMatches.AnyAsync(m => m.IdEvent == idEvent);
            var hasIndividual = await _context.IndividualMatches.AnyAsync(m => m.IdEvent == idEvent);

            if (hasTeams)
                await AutoAdvanceTeamEventAsync(ev, force);

            if (hasIndividual)
                await AutoAdvanceIndividualEventAsync(ev, force);

            await _context.SaveChangesAsync();
        }

        private async Task AutoAdvanceTeamEventAsync(Event ev, bool force)
        {
            var all = await _context.TeamMatches.Where(m => m.IdEvent == ev.IdEvent).ToListAsync();
            if (!all.Any()) return;

            switch (ev.systems)
            {
                case SelectionSystems.RoundRobin:
                    // Round robin is complete when all generated matches are finished.
                    return;

                case SelectionSystems.GroupStage:
                    await AdvanceGroupStageTeamsAsync(ev, all, force);
                    return;

                case SelectionSystems.SwissSystem:
                    await AdvanceSwissTeamsAsync(ev, all, force);
                    return;

                case SelectionSystems.DoubleElimination:
                    await AdvanceDoubleEliminationTeamsAsync(ev, all, force);
                    return;

                case SelectionSystems.PlayOff:
                case SelectionSystems.OlympicSystem:
                case SelectionSystems.KnockoutSystem:
                case SelectionSystems.Final:
                default:
                    await AdvanceSingleEliminationTeamsAsync(ev, all, force);
                    return;
            }
        }

        private async Task AutoAdvanceIndividualEventAsync(Event ev, bool force)
        {
            var all = await _context.IndividualMatches.Where(m => m.IdEvent == ev.IdEvent).ToListAsync();
            if (!all.Any()) return;

            switch (ev.systems)
            {
                case SelectionSystems.SwissSystem:
                    await AdvanceSwissIndividualsAsync(ev, all, force);
                    return;

                case SelectionSystems.DoubleElimination:
                    await AdvanceDoubleEliminationIndividualsAsync(ev, all, force);
                    return;

                case SelectionSystems.GroupStage:
                    await AdvanceGroupStageIndividualsAsync(ev, all, force);
                    return;

                case SelectionSystems.QualificationByStandards:
                    return;

                case SelectionSystems.RoundRobin:
                    return;

                case SelectionSystems.PlayOff:
                case SelectionSystems.OlympicSystem:
                case SelectionSystems.KnockoutSystem:
                case SelectionSystems.Final:
                default:
                    await AdvanceSingleEliminationIndividualsAsync(ev, all, force);
                    return;
            }
        }

        private async Task AdvanceSingleEliminationTeamsAsync(Event ev, List<TeamMatch> all, bool force)
        {
            var maxTour = all.Max(m => m.Tour);
            var round = all.Where(m => m.Tour == maxTour).ToList();

            if (!force && !round.All(IsFinished)) return;
            if (await _context.TeamMatches.AnyAsync(m => m.IdEvent == ev.IdEvent && m.Tour == maxTour + 1)) return;

            var winners = round.Select(GetWinnerTeam).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

            if (winners.Count <= 1)
            {
                ev.DataEnd ??= DateTime.UtcNow;
                return;
            }

            AddTeamPairs(ev.IdEvent, winners, maxTour + 1, 0, "MAIN", round.FirstOrDefault()?.LocationName, round.FirstOrDefault()?.loginJudge);
        }

        private async Task AdvanceSingleEliminationIndividualsAsync(Event ev, List<IndividualMatch> all, bool force)
        {
            var maxTour = all.Max(m => m.Tour);
            var round = all.Where(m => m.Tour == maxTour).ToList();

            if (!force && !round.All(IsFinished)) return;
            if (await _context.IndividualMatches.AnyAsync(m => m.IdEvent == ev.IdEvent && m.Tour == maxTour + 1)) return;

            var winners = round.Select(GetWinnerIndividual).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

            if (winners.Count <= 1)
            {
                ev.DataEnd ??= DateTime.UtcNow;
                return;
            }

            AddIndividualPairs(ev.IdEvent, winners, maxTour + 1, 0, "MAIN", round.FirstOrDefault()?.LocationName, round.FirstOrDefault()?.loginJudge);
        }

        private async Task AdvanceGroupStageTeamsAsync(Event ev, List<TeamMatch> all, bool force)
        {
            var groupMatches = all.Where(m => (m.Group ?? 1) > 0).ToList();
            if (!groupMatches.Any()) return;

            if (!force && !groupMatches.All(IsFinished)) return;

            var playoffTour = all.Max(m => m.Tour) + 1;
            if (await _context.TeamMatches.AnyAsync(m => m.IdEvent == ev.IdEvent && (m.Group ?? 0) == 0 && m.Tour == playoffTour)) return;

            var qualifiers = new List<string>();

            foreach (var group in groupMatches.GroupBy(m => m.Group ?? 1))
            {
                var standings = BuildTeamStandings(group.ToList());
                qualifiers.AddRange(standings.OrderByDescending(s => s.Points).ThenByDescending(s => s.ScoreDiff).ThenByDescending(s => s.ScoreFor).Take(2).Select(s => s.Participant));
            }

            if (qualifiers.Count <= 1)
            {
                ev.DataEnd ??= DateTime.UtcNow;
                return;
            }

            AddTeamPairs(ev.IdEvent, qualifiers.Distinct().ToList(), playoffTour, 0, "PLAYOFF", groupMatches.FirstOrDefault()?.LocationName, groupMatches.FirstOrDefault()?.loginJudge);
        }

        private async Task AdvanceGroupStageIndividualsAsync(Event ev, List<IndividualMatch> all, bool force)
        {
            var groupMatches = all.Where(m => (m.Group ?? 1) > 0).ToList();
            if (!groupMatches.Any()) return;
            if (!force && !groupMatches.All(IsFinished)) return;

            var playoffTour = all.Max(m => m.Tour) + 1;
            if (await _context.IndividualMatches.AnyAsync(m => m.IdEvent == ev.IdEvent && (m.Group ?? 0) == 0 && m.Tour == playoffTour)) return;

            var qualifiers = new List<string>();

            foreach (var group in groupMatches.GroupBy(m => m.Group ?? 1))
            {
                var standings = BuildIndividualStandings(group.ToList());
                qualifiers.AddRange(standings.OrderByDescending(s => s.Points).ThenByDescending(s => s.ScoreDiff).ThenByDescending(s => s.ScoreFor).Take(2).Select(s => s.ParticipantLogin ?? s.Participant));
            }

            if (qualifiers.Count <= 1)
            {
                ev.DataEnd ??= DateTime.UtcNow;
                return;
            }

            AddIndividualPairs(ev.IdEvent, qualifiers.Distinct().ToList(), playoffTour, 0, "PLAYOFF", groupMatches.FirstOrDefault()?.LocationName, groupMatches.FirstOrDefault()?.loginJudge);
        }

        private async Task AdvanceSwissTeamsAsync(Event ev, List<TeamMatch> all, bool force)
        {
            var maxTour = all.Max(m => m.Tour);
            var currentRound = all.Where(m => m.Tour == maxTour).ToList();
            if (!force && !currentRound.All(IsFinished)) return;

            var participants = all.SelectMany(m => new[] { m.NameFirstTeam, m.NameSecondTeam }).Distinct().ToList();
            var maxRounds = Math.Max(1, (int)Math.Ceiling(Math.Log2(Math.Max(2, participants.Count))) + 1);
            if (maxTour >= maxRounds)
            {
                ev.DataEnd ??= DateTime.UtcNow;
                return;
            }

            if (await _context.TeamMatches.AnyAsync(m => m.IdEvent == ev.IdEvent && m.Tour == maxTour + 1)) return;

            var standings = BuildTeamStandings(all).OrderByDescending(s => s.Points).ThenByDescending(s => s.ScoreDiff).ThenByDescending(s => s.ScoreFor).Select(s => s.Participant).ToList();
            var pairs = BuildSwissPairs(standings, all.Select(m => PairKey(m.NameFirstTeam, m.NameSecondTeam)).ToHashSet());

            foreach (var p in pairs)
            {
                _context.TeamMatches.Add(new TeamMatch
                {
                    IdEvent = ev.IdEvent,
                    NameFirstTeam = p.Item1,
                    NameSecondTeam = p.Item2,
                    Tour = maxTour + 1,
                    Group = 0,
                    StatusMatch = StatusMatch.Upcoming,
                    LocationName = currentRound.FirstOrDefault()?.LocationName,
                    loginJudge = currentRound.FirstOrDefault()?.loginJudge,
                    AddInformation = "bracket=SWISS;auto=true"
                });
            }
        }

        private async Task AdvanceSwissIndividualsAsync(Event ev, List<IndividualMatch> all, bool force)
        {
            var maxTour = all.Max(m => m.Tour);
            var currentRound = all.Where(m => m.Tour == maxTour).ToList();
            if (!force && !currentRound.All(IsFinished)) return;

            var participants = all.SelectMany(m => new[] { m.loginFirstAthlete, m.loginSecondAthlete }).Distinct().ToList();
            var maxRounds = Math.Max(1, (int)Math.Ceiling(Math.Log2(Math.Max(2, participants.Count))) + 1);
            if (maxTour >= maxRounds)
            {
                ev.DataEnd ??= DateTime.UtcNow;
                return;
            }

            if (await _context.IndividualMatches.AnyAsync(m => m.IdEvent == ev.IdEvent && m.Tour == maxTour + 1)) return;

            var standings = BuildIndividualStandings(all).OrderByDescending(s => s.Points).ThenByDescending(s => s.ScoreDiff).ThenByDescending(s => s.ScoreFor).Select(s => s.ParticipantLogin ?? s.Participant).ToList();
            var pairs = BuildSwissPairs(standings, all.Select(m => PairKey(m.loginFirstAthlete, m.loginSecondAthlete)).ToHashSet());

            foreach (var p in pairs)
            {
                _context.IndividualMatches.Add(new IndividualMatch
                {
                    IdEvent = ev.IdEvent,
                    loginFirstAthlete = p.Item1,
                    loginSecondAthlete = p.Item2,
                    Tour = maxTour + 1,
                    Group = 0,
                    StatusMatch = StatusMatch.Upcoming,
                    LocationName = currentRound.FirstOrDefault()?.LocationName,
                    loginJudge = currentRound.FirstOrDefault()?.loginJudge,
                    AddInformation = "bracket=SWISS;auto=true"
                });
            }
        }

        private async Task AdvanceDoubleEliminationTeamsAsync(Event ev, List<TeamMatch> all, bool force)
        {
            var maxTour = all.Max(m => m.Tour);
            var current = all.Where(m => m.Tour == maxTour).ToList();
            if (!force && !current.All(IsFinished)) return;
            if (await _context.TeamMatches.AnyAsync(m => m.IdEvent == ev.IdEvent && m.Tour == maxTour + 1)) return;

            var wb = current.Where(m => GetInfo(m.AddInformation, "bracket") != "LB").ToList();
            var lb = current.Where(m => GetInfo(m.AddInformation, "bracket") == "LB").ToList();

            var wbWinners = wb.Select(GetWinnerTeam).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            var wbLosers = wb.Select(GetLoserTeam).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            var lbWinners = lb.Select(GetWinnerTeam).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

            if (wbWinners.Count == 1 && lbWinners.Count <= 1 && wbLosers.Count == 0)
            {
                ev.DataEnd ??= DateTime.UtcNow;
                return;
            }

            if (wbWinners.Count > 1)
                AddTeamPairs(ev.IdEvent, wbWinners, maxTour + 1, 0, "WB", current.FirstOrDefault()?.LocationName, current.FirstOrDefault()?.loginJudge);

            var lower = lbWinners.Concat(wbLosers).Distinct().ToList();
            if (lower.Count > 1)
                AddTeamPairs(ev.IdEvent, lower, maxTour + 1, 0, "LB", current.FirstOrDefault()?.LocationName, current.FirstOrDefault()?.loginJudge);

            if (wbWinners.Count == 1 && lower.Count == 1 && wbWinners[0] != lower[0])
                AddTeamPairs(ev.IdEvent, new List<string> { wbWinners[0], lower[0] }, maxTour + 1, 0, "FINAL", current.FirstOrDefault()?.LocationName, current.FirstOrDefault()?.loginJudge);
        }

        private async Task AdvanceDoubleEliminationIndividualsAsync(Event ev, List<IndividualMatch> all, bool force)
        {
            var maxTour = all.Max(m => m.Tour);
            var current = all.Where(m => m.Tour == maxTour).ToList();
            if (!force && !current.All(IsFinished)) return;
            if (await _context.IndividualMatches.AnyAsync(m => m.IdEvent == ev.IdEvent && m.Tour == maxTour + 1)) return;

            var wb = current.Where(m => GetInfo(m.AddInformation, "bracket") != "LB").ToList();
            var lb = current.Where(m => GetInfo(m.AddInformation, "bracket") == "LB").ToList();

            var wbWinners = wb.Select(GetWinnerIndividual).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            var wbLosers = wb.Select(GetLoserIndividual).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            var lbWinners = lb.Select(GetWinnerIndividual).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

            if (wbWinners.Count > 1)
                AddIndividualPairs(ev.IdEvent, wbWinners, maxTour + 1, 0, "WB", current.FirstOrDefault()?.LocationName, current.FirstOrDefault()?.loginJudge);

            var lower = lbWinners.Concat(wbLosers).Distinct().ToList();
            if (lower.Count > 1)
                AddIndividualPairs(ev.IdEvent, lower, maxTour + 1, 0, "LB", current.FirstOrDefault()?.LocationName, current.FirstOrDefault()?.loginJudge);

            if (wbWinners.Count == 1 && lower.Count == 1 && wbWinners[0] != lower[0])
                AddIndividualPairs(ev.IdEvent, new List<string> { wbWinners[0], lower[0] }, maxTour + 1, 0, "FINAL", current.FirstOrDefault()?.LocationName, current.FirstOrDefault()?.loginJudge);
        }

        private void AddTeamPairs(long idEvent, List<string> participants, int tour, int group, string bracket, string? location, string? judge)
        {
            var pairs = PairSequential(participants);

            foreach (var p in pairs)
            {
                _context.TeamMatches.Add(new TeamMatch
                {
                    IdEvent = idEvent,
                    NameFirstTeam = p.Item1,
                    NameSecondTeam = p.Item2,
                    Tour = tour,
                    Group = group,
                    StatusMatch = StatusMatch.Upcoming,
                    LocationName = location,
                    loginJudge = judge,
                    AddInformation = $"bracket={bracket};auto=true"
                });
            }
        }

        private void AddIndividualPairs(long idEvent, List<string> participants, int tour, int group, string bracket, string? location, string? judge)
        {
            var pairs = PairSequential(participants);

            foreach (var p in pairs)
            {
                _context.IndividualMatches.Add(new IndividualMatch
                {
                    IdEvent = idEvent,
                    loginFirstAthlete = p.Item1,
                    loginSecondAthlete = p.Item2,
                    Tour = tour,
                    Group = group,
                    StatusMatch = StatusMatch.Upcoming,
                    LocationName = location,
                    loginJudge = judge,
                    AddInformation = $"bracket={bracket};auto=true"
                });
            }
        }

        private List<Tuple<string, string>> PairSequential(List<string> participants)
        {
            var clean = participants
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var result = new List<Tuple<string, string>>();

            for (var i = 0; i + 1 < clean.Count; i += 2)
            {
                result.Add(Tuple.Create(clean[i], clean[i + 1]));
            }

            return result;
        }

        private (int First, int Second)? ParseScore(string? score)
        {
            if (string.IsNullOrWhiteSpace(score))
                return null;

            var match = Regex.Match(score, @"(-?\d+)\s*[:\-]\s*(-?\d+)");

            if (!match.Success)
                return null;

            return (
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value)
            );
        }

        private List<Tuple<string, string>> BuildSwissPairs(List<string> standings, HashSet<string> previousPairs)
        {
            var queue = standings.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            var result = new List<Tuple<string, string>>();

            while (queue.Count >= 2)
            {
                var first = queue[0];
                queue.RemoveAt(0);

                var opponentIndex = queue.FindIndex(x => !previousPairs.Contains(PairKey(first, x)));
                if (opponentIndex < 0) opponentIndex = 0;

                var second = queue[opponentIndex];
                queue.RemoveAt(opponentIndex);

                result.Add(Tuple.Create(first, second));
            }

            return result;
        }

        private List<EventStandingDto> BuildStandings(List<EventMatchCatalogDto> matches)
        {
            var standings = new Dictionary<string, EventStandingDto>();

            foreach (var m in matches.Where(m => m.Status == "Finished"))
            {
                AddStandingResult(standings, m.FirstParticipant, m.FirstLogin, m.SecondParticipant, m.SecondLogin, m.Score, m.Winner, m.Group ?? 0);
            }

            return standings.Values
                .OrderBy(s => s.Group)
                .ThenByDescending(s => s.Points)
                .ThenByDescending(s => s.ScoreDiff)
                .ThenByDescending(s => s.ScoreFor)
                .ThenBy(s => s.Participant)
                .ToList();
        }

        private List<EventStandingDto> BuildTeamStandings(List<TeamMatch> matches)
        {
            var standings = new Dictionary<string, EventStandingDto>();

            foreach (var m in matches.Where(IsFinished))
            {
                var info = ParseInfo(m.AddInformation);
                AddStandingResult(standings, m.NameFirstTeam, null, m.NameSecondTeam, null, info.GetValueOrDefault("score"), info.GetValueOrDefault("winner"), m.Group ?? 0);
            }

            return standings.Values.ToList();
        }

        private List<EventStandingDto> BuildIndividualStandings(List<IndividualMatch> matches)
        {
            var standings = new Dictionary<string, EventStandingDto>();

            foreach (var m in matches.Where(IsFinished))
            {
                var info = ParseInfo(m.AddInformation);
                AddStandingResult(standings, m.loginFirstAthlete, m.loginFirstAthlete, m.loginSecondAthlete, m.loginSecondAthlete, info.GetValueOrDefault("score"), info.GetValueOrDefault("winner"), m.Group ?? 0);
            }

            return standings.Values.ToList();
        }

        private void AddStandingResult(Dictionary<string, EventStandingDto> standings, string first, string? firstLogin, string second, string? secondLogin, string? score, string? winner, int group)
        {
            if (!standings.ContainsKey(first))
                standings[first] = new EventStandingDto { Participant = first, ParticipantLogin = firstLogin, Group = group };
            if (!standings.ContainsKey(second))
                standings[second] = new EventStandingDto { Participant = second, ParticipantLogin = secondLogin, Group = group };

            var a = standings[first];
            var b = standings[second];

            a.Played++;
            b.Played++;

            var parsed = ParseScore(score);

            if (parsed.HasValue)
            {
                a.ScoreFor += parsed.Value.Item1;
                a.ScoreAgainst += parsed.Value.Item2;
                b.ScoreFor += parsed.Value.Item2;
                b.ScoreAgainst += parsed.Value.Item1;

                if (parsed.Value.Item1 > parsed.Value.Item2)
                {
                    a.Wins++; a.Points += 3;
                    b.Losses++;
                    return;
                }

                if (parsed.Value.Item2 > parsed.Value.Item1)
                {
                    b.Wins++; b.Points += 3;
                    a.Losses++;
                    return;
                }

                a.Draws++; b.Draws++;
                a.Points++; b.Points++;
                return;
            }

            if (!string.IsNullOrWhiteSpace(winner))
            {
                if (winner == first || winner == firstLogin)
                {
                    a.Wins++; a.Points += 3; b.Losses++;
                }
                else if (winner == second || winner == secondLogin)
                {
                    b.Wins++; b.Points += 3; a.Losses++;
                }
            }
        }

        private string? GetWinnerTeam(TeamMatch m)
        {
            var info = ParseInfo(m.AddInformation);
            var winner = info.GetValueOrDefault("winner");
            if (!string.IsNullOrWhiteSpace(winner)) return winner;

            var score = ParseScore(info.GetValueOrDefault("score"));
            if (!score.HasValue) return null;
            if (score.Value.Item1 > score.Value.Item2) return m.NameFirstTeam;
            if (score.Value.Item2 > score.Value.Item1) return m.NameSecondTeam;
            return null;
        }

        private string? GetLoserTeam(TeamMatch m)
        {
            var winner = GetWinnerTeam(m);
            if (string.IsNullOrWhiteSpace(winner)) return null;
            if (winner == m.NameFirstTeam) return m.NameSecondTeam;
            if (winner == m.NameSecondTeam) return m.NameFirstTeam;
            return null;
        }

        private string? GetWinnerIndividual(IndividualMatch m)
        {
            var info = ParseInfo(m.AddInformation);
            var winner = info.GetValueOrDefault("winner");
            if (!string.IsNullOrWhiteSpace(winner)) return winner;

            var score = ParseScore(info.GetValueOrDefault("score"));
            if (!score.HasValue) return null;
            if (score.Value.Item1 > score.Value.Item2) return m.loginFirstAthlete;
            if (score.Value.Item2 > score.Value.Item1) return m.loginSecondAthlete;
            return null;
        }

        private string? GetLoserIndividual(IndividualMatch m)
        {
            var winner = GetWinnerIndividual(m);
            if (string.IsNullOrWhiteSpace(winner)) return null;
            if (winner == m.loginFirstAthlete) return m.loginSecondAthlete;
            if (winner == m.loginSecondAthlete) return m.loginFirstAthlete;
            return null;
        }

        private bool IsFinished(TeamMatch m) => m.StatusMatch == StatusMatch.Finished;
        private bool IsFinished(IndividualMatch m) => m.StatusMatch == StatusMatch.Finished;

        private string BuildResultText(MatchResultSubmitDto dto)
        {
            return $"score={Sanitize(dto.Score)};winner={Sanitize(dto.Winner)};notes={Sanitize(dto.Notes)};updatedBy={Sanitize(dto.Login)};updatedAt={DateTime.UtcNow:O}";
        }

        private string MergeInfo(string? oldInfo, string newInfo)
        {
            var old = ParseInfo(oldInfo);
            var incoming = ParseInfo(newInfo);

            foreach (var kv in incoming)
                old[kv.Key] = kv.Value;

            return string.Join(";", old.Select(kv => $"{kv.Key}={kv.Value}"));
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

                if (!string.IsNullOrWhiteSpace(key))
                    result[key] = val;
            }

            return result;
        }

        private string? GetInfo(string? value, string key)
        {
            return ParseInfo(value).GetValueOrDefault(key);
        }

        private string PairKey(string a, string b)
        {
            return string.CompareOrdinal(a, b) <= 0 ? $"{a}__{b}" : $"{b}__{a}";
        }

        private string Sanitize(string? value)
        {
            return (value ?? "").Replace(";", ",").Replace("=", ":").Trim();
        }

        private string ResolveEventStatus(Event ev, List<EventMatchCatalogDto> matches)
        {
            if (matches.Any(m => m.Status == "Live")) return "Live";
            if (matches.Count > 0 && matches.All(m => m.Status == "Finished")) return "Finished";
            if (ev.DataStart > DateTime.UtcNow) return "Upcoming";
            return "Upcoming";
        }

        private async Task<bool> CanEditMatchAsync(string matchType, long matchId, string? login, string? role)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(role))
                return false;

            matchType = matchType.Trim().ToLower();

            if (matchType == "team")
            {
                var m = await _context.TeamMatches.AsNoTracking().FirstOrDefaultAsync(x => x.IdTeamMatch == matchId);
                return m != null && await CanEditTeamMatchAsync(m, login, role);
            }

            if (matchType == "individual")
            {
                var m = await _context.IndividualMatches.AsNoTracking().FirstOrDefaultAsync(x => x.IdIndividualMatch == matchId);
                return m != null && await CanEditIndividualMatchAsync(m, login, role);
            }

            if (matchType == "extreme")
            {
                var m = await _context.ExtremeMatches.AsNoTracking().FirstOrDefaultAsync(x => x.IdExtremeMatches == matchId);
                return m != null && await CanEditExtremeMatchAsync(m, login, role);
            }

            return false;
        }

        private async Task<bool> CanManageEventAsync(long idEvent, string? login, string? role)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(role))
                return false;

            if (role == "Judge")
            {
                var hasJudgeMatch =
                    await _context.TeamMatches.AnyAsync(m => m.IdEvent == idEvent && m.loginJudge == login) ||
                    await _context.IndividualMatches.AnyAsync(m => m.IdEvent == idEvent && m.loginJudge == login) ||
                    await _context.ExtremeMatches.AnyAsync(m => m.IdEvent == idEvent && m.loginJudge == login);

                return hasJudgeMatch;
            }

            if (role == "Organization")
                return await IsRelatedOrganizationAsync(idEvent, login, role);

            return false;
        }

        private async Task<bool> CanEditTeamMatchAsync(TeamMatch m, string? login, string? role)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(role)) return false;

            if (role == "Judge" && !string.IsNullOrWhiteSpace(m.loginJudge) && m.loginJudge == login)
                return true;

            if (role == "Organization")
            {
                var relatedTeams = await _context.OrganizationTeams
                    .AsNoTracking()
                    .Where(x => x.LoginOrganization == login)
                    .Select(x => x.NameComand)
                    .ToListAsync();

                return relatedTeams.Contains(m.NameFirstTeam) || relatedTeams.Contains(m.NameSecondTeam);
            }

            return false;
        }

        private async Task<bool> CanEditIndividualMatchAsync(IndividualMatch m, string? login, string? role)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(role)) return false;

            if (role == "Judge" && !string.IsNullOrWhiteSpace(m.loginJudge) && m.loginJudge == login)
                return true;

            if (role == "Organization")
                return await IsRelatedOrganizationAsync(m.IdEvent, login, role);

            return false;
        }

        private async Task<bool> CanEditExtremeMatchAsync(ExtremeMatch m, string? login, string? role)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(role)) return false;

            if (role == "Judge" && !string.IsNullOrWhiteSpace(m.loginJudge) && m.loginJudge == login)
                return true;

            if (role == "Organization")
                return await IsRelatedOrganizationAsync(m.IdEvent, login, role);

            return false;
        }

        private async Task<bool> IsRelatedOrganizationAsync(long idEvent, string? login, string? role)
        {
            if (string.IsNullOrWhiteSpace(login) || role != "Organization")
                return false;

            var orgTeams = await _context.OrganizationTeams
                .AsNoTracking()
                .Where(x => x.LoginOrganization == login)
                .Select(x => x.NameComand)
                .ToListAsync();

            if (!orgTeams.Any()) return false;

            var hasTeamMatch = await _context.TeamMatches
                .AsNoTracking()
                .AnyAsync(m => m.IdEvent == idEvent && (orgTeams.Contains(m.NameFirstTeam) || orgTeams.Contains(m.NameSecondTeam)));

            if (hasTeamMatch) return true;

            var extremeIds = await _context.EMatchesTeam
                .AsNoTracking()
                .Where(x => orgTeams.Contains(x.NameTeam))
                .Select(x => x.IdExtremeMatches)
                .ToListAsync();

            if (!extremeIds.Any()) return false;

            return await _context.ExtremeMatches
                .AsNoTracking()
                .AnyAsync(m => m.IdEvent == idEvent && extremeIds.Contains(m.IdExtremeMatches));
        }

        private string ResolveEditReason(string? loginJudge, string? login, string? role)
        {
            if (role == "Judge" && loginJudge == login) return "AssignedJudge";
            if (role == "Organization") return "RelatedOrganization";
            return "ViewOnly";
        }
    }
}
