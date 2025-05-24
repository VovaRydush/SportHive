using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SaveTeamMatch : ISaveMatchInfo
    {
        private readonly IEnterDataMatches _dataMatches;
        private static long _counter = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public SaveTeamMatch(IEnterDataMatches dataMatches)
        {
            _dataMatches = dataMatches;
        }
        public async Task SaveMatch(AppDbContext appDbContext,Matchs matchs,string Team1, string Team2,long IdEvent)
        {
            var IdTeamMatchs = Interlocked.Increment(ref _counter);
            var entity = new TeamMatch
            {
                IdTeamMatch = IdTeamMatchs,
                IdEvent = IdEvent,
                StatusMatch = StatusMatch.Upcoming,
                NameFirstTeam = Team1,
                NameSecondTeam = Team2,
                Group = matchs.Group ?? -1,
                Tour = matchs.tour,
                AddInformation = matchs.AddInformation ?? ""
            };
          
           await _dataMatches.SaveMatches(new TeamInfo
            {
                idMatch = IdTeamMatchs,
                NameDesipline = matchs.NameSport,
                Group = matchs.Group,
                Tour = matchs.tour
            });
            appDbContext.TeamMatches.Add(entity);
        }
        public Task SaveMatch(AppDbContext appDbContext, Matchs matchs, long IdExtremeMatches, string TypeMatch)
        {
            throw new NotImplementedException();
        }
    }
}