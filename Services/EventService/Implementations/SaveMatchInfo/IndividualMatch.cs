using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SaveIndividualMatch : ISaveMatchInfo
    {
        private readonly AppDbContext _appDbContext;
        private readonly IEnterDataMatches _dataMatches;
        private static long _counter = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public SaveIndividualMatch(AppDbContext appDbContext,IEnterDataMatches dataMatches)
        {
            _dataMatches = dataMatches;
            _appDbContext = appDbContext;
        }
        public void SaveMatch(Matchs matchs, string athlete1, string athlete2,long IdEvent)
        {
            var IdIndividualMatchs = Interlocked.Increment(ref _counter);
            var entity = new IndividualMatch
            {
                IdIndividualMatch  = IdIndividualMatchs,
                IdEvent = IdEvent,
                loginFirstAthlete = athlete1,
                loginSecondAthlete = athlete2,
                StatusMatch = StatusMatch.Upcoming,
                Tour = matchs.tour,
                AddInformation = matchs.AddInformation ?? ""
            };
            _dataMatches.SaveMatches(new TwoPlayerInfo
            {
                FullNamePlayer1 = athlete1,
                FullNamePlayer2 = athlete2,
                idMatch = IdEvent
            });
            _appDbContext.IndividualMatches.Add(entity);
        }
    }
}