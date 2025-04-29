using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.VisualBasic;
using SportHive.Services.Interfaces;
using StackExchange.Redis;

namespace SportHive.Implementations
{
    public class SaveMatchInfo : ISaveMatchInfo
    {
        private readonly AppDbContext _dbcontext;
        public SaveMatchInfo(AppDbContext appDbContext)
        {
            _dbcontext = appDbContext;
        }
        public async Task SaveTeamMatch(MatchsAbstractionDto matchs,string Team1, string Team2,long IdEvent)
        {
            var entity = new TeamMatch
            {
                IdEvent = IdEvent,
                StatusMatch = StatusMatch.Upcoming,
                NameFirstTeam = Team1,
                NameSecondTeam = Team2,
                Tour = matchs.tour,
                AddInformation = matchs.AddInformation
            };
            _dbcontext.TeamMatches.Add(entity);
            await Task.CompletedTask;
        }

        public async Task SaveIndividualMatch(MatchsAbstractionDto matchs, string athlete1, string athlete2,long IdEvent)
        {
            var entity = new IndividualMatch
            {
                IdEvent = IdEvent,
                loginFirstAthlete = athlete1,
                loginSecondAthlete = athlete2,
                StatusMatch = StatusMatch.Upcoming,
                Tour = matchs.tour,
                AddInformation = matchs.AddInformation
            };
            _dbcontext.IndividualMatches.Add(entity);
            await Task.CompletedTask;
        }

        public async Task SaveExtremeMatchTeam(MatchsAbstractionDto matchs,long IdExtremeMatches)
        {
            var entities = new List<EMatchesTeam>();
             foreach (var team in matchs.Entitys)
             {
                var entity = new EMatchesTeam
                {
                    IdExtremeMatches = IdExtremeMatches,
                    NameTeam = team.ToString()
                };
                entities.Add(entity);
             }
             _dbcontext.AddRange(entities);
             await Task.CompletedTask;
        }

        public Task SaveExtremeMatchIndividual(MatchsAbstractionDto matchs)
        {
            throw new NotImplementedException();
        }
    }
}