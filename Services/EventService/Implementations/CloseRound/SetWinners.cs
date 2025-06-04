using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SetWinners : ISetWinner
    {
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        private readonly ISetResultMatch _setResultMatch;
        public SetWinners(IMongoDbService mongoDbService,ISetResultMatch setResultMatch)
        {
            _setResultMatch = setResultMatch;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
        }
        public async Task SetWinnerBoxStruggle(BoxWinnerDto winner)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                  Builders<MatchEvents>.Filter.Eq(x => x.idMatch, winner.idMatch)
              );
            var update = Builders<MatchEvents>.Update.Combine(
                Builders<MatchEvents>.Update.Set("winner", new WinStruggleResult
                {
                    FullNamePlayer = winner.FullNamePlayer,
                    loginPlayer = winner.loginWinner,
                    round = winner.round,
                    win = winner.win
                })
            );
            await _matchEvents.UpdateOneAsync(filter, update);
        }
        public async Task SetWinnerChessCheckers(BoardWinner winner)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                     Builders<MatchEvents>.Filter.Eq(x => x.idMatch, winner.idMatch)
                 );
            var update = Builders<MatchEvents>.Update.Combine(
                Builders<MatchEvents>.Update.Push("Result", winner)
            );
            await _matchEvents.UpdateOneAsync(filter, update);
        }
    }
}