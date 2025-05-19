using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SaveTeamMatch : ISaveMatchInfo
    {
        private readonly AppDbContext _appDbContext;
        private readonly IEnterDataMatches _dataMatches;
        private static long _counter = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public SaveTeamMatch(AppDbContext appDbContext, IEnterDataMatches dataMatches)
        {
            _dataMatches = dataMatches;
            _appDbContext = appDbContext;
        }
        public void SaveMatch(Matchs matchs,string Team1, string Team2,long IdEvent)
        {
            var IdTeamMatchs = Interlocked.Increment(ref _counter);
            var entity = new TeamMatch
            {
                IdTeamMatch = IdTeamMatchs,
                IdEvent = IdEvent,
                StatusMatch = StatusMatch.Upcoming,
                NameFirstTeam = Team1,
                NameSecondTeam = Team2,
                Tour = matchs.tour,
                AddInformation = matchs.AddInformation ?? ""
            };
            _dataMatches.SaveMatches(new TeamInfo
            {
                idMatch = IdTeamMatchs,
                NameDesipline = matchs.NameSport,
                Tour = matchs.tour
            });
            _appDbContext.TeamMatches.Add(entity);
        }
    }
}