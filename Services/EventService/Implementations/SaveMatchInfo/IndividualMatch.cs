using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SaveIndividualMatch : ISaveMatchInfo
    {
        private readonly IEnterDataMatches _dataMatches;
        private static long _counter = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public SaveIndividualMatch(IEnterDataMatches dataMatches)
        {
            _dataMatches = dataMatches;
        }
        public async Task SaveMatch(AppDbContext appDbContext,Matchs matchs, string athlete1, string athlete2,long IdEvent)
        {
            var IdIndividualMatchs = Interlocked.Increment(ref _counter);
            var entity = new IndividualMatch
            {
                IdIndividualMatch  = IdIndividualMatchs,
                IdEvent = IdEvent,
                loginFirstAthlete = athlete1,
                loginJudge = matchs.loginJudge,
                loginSecondAthlete = athlete2,
                StatusMatch = StatusMatch.Upcoming,
                Tour = matchs.tour,
                AddInformation = matchs.AddInformation ?? ""
            };
            await _dataMatches.SaveMatches(new TwoPlayerInfo
            {
                FullNamePlayer1 = athlete1,
                FullNamePlayer2 = athlete2,
                tour = matchs.tour,
                loginJudge = matchs.loginJudge,
                NameDesipline = matchs.NameSport,
                idMatch = IdIndividualMatchs
            });
            appDbContext.IndividualMatches.Add(entity);
        }

        public Task SaveMatch(AppDbContext appDbContext,Matchs matchs, long IdExtremeMatches, string TypeMatch)
        {
            throw new NotImplementedException();
        }
    }
}