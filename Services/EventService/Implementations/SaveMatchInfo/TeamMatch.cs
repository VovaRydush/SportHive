using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SaveTeamMatch : ISaveMatchInfo
    {
        private readonly AppDbContext _appDbContext;
        public SaveTeamMatch(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public void SaveMatch(Matchs matchs,string Team1, string Team2,long IdEvent)
        {
            var entity = new TeamMatch
            {
                IdEvent = IdEvent,
                StatusMatch = StatusMatch.Upcoming,
                NameFirstTeam = Team1,
                NameSecondTeam = Team2,
                Tour = matchs.tour,
                AddInformation = matchs.AddInformation ?? ""
            };
            _appDbContext.TeamMatches.Add(entity);
        }
    }
}