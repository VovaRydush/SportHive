using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SaveIndividualMatch : ISaveMatchInfo
    {
        private readonly AppDbContext _appDbContext;
        public SaveIndividualMatch(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public void SaveMatch(Matchs matchs, string athlete1, string athlete2,long IdEvent)
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
            _appDbContext.IndividualMatches.Add(entity);
        }
    }
}