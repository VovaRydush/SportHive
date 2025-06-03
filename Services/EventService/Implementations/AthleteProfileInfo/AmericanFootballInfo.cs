using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using SportHive.Services.Interfaces;
using StackExchange.Redis;

namespace SportHive.Implementations
{
    public class AmericanFootballInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public AmericanFootballInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
            _userUps = userUps;
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var update = Builders<AthleteProfile>.Update
                .Set("SportStats.Matches", _userUps.CountMatchs(athlet.loginPlayer))
                .Set("SportStats.Touchdowns", CountTouchdowns(athlet.loginPlayer))
                .Set("SportStats.Interceptions", CountInterceptions(athlet.loginPlayer))
                .Set("SportStats.Tackles", CountTackles(athlet.loginPlayer))
                .Set("SportStats.Wins", _userUps.CountWinTeam(athlet))
                .Set("SportStats.Losses", _userUps.CountLossTeam(athlet))
                .Set("SportStats.Draws",_userUps.CountDrawIndividual(athlet.loginPlayer))
                .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        public async Task<int> CountTouchdowns(string loginPlayer)
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("NameDesipline", "AmericanFootball")),
                new BsonDocument("$unwind", "$touchdowns"),
                new BsonDocument("$match", new BsonDocument("touchdowns.loginPlayer", loginPlayer)),
                new BsonDocument("$count", "totalTouchdowns")
            };

            var result = await _matchEvents
                .Aggregate<BsonDocument>(pipeline)
                .FirstOrDefaultAsync();

            return result?["totalTouchdowns"].AsInt32 ?? 0;

        }
        public async Task<int> CountInterceptions(string loginPlayer)
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("NameDesipline", "AmericanFootball")),
                new BsonDocument("$unwind", "$twoPlayersMoves"),
                new BsonDocument("$match", new BsonDocument
                {
                    { "twoPlayersMoves.FullNamePlayer1", loginPlayer },
                    { "twoPlayersMoves.typeMove", (int)TypeMovePlayer.Interception }
                }),
                new BsonDocument("$count", "totalInterceptions")
            };

            var result = await _matchEvents
                .Aggregate<BsonDocument>(pipeline)
                .FirstOrDefaultAsync();

            return result?["totalInterceptions"].AsInt32 ?? 0;
        }
        public async Task<int> CountTackles(string loginPlayer)
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("NameDesipline", "AmericanFootball")),
                new BsonDocument("$unwind", "$playMoves"),
                new BsonDocument("$match", new BsonDocument
                {
                    { "playMoves.loginPlayer", loginPlayer },
                    { "playMoves.typeMove", (int)TypeMove.Tackles } 
                }),
                new BsonDocument("$count", "tacklesCount")
            };

            var result = await _matchEvents
                .Aggregate<BsonDocument>(pipeline)
                .FirstOrDefaultAsync();

            return result?["tacklesCount"].AsInt32 ?? 0;
        }
    }
}