using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class ArcheryInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public ArcheryInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var archery = await GetArcheryByMatchId(athlet.idMatch);
            var update = Builders<AthleteProfile>.Update
               .Set("SportStats.Matches", _userUps.CountMatchs(athlet.loginPlayer))
               .Set("SportStats.Competitions", archery.CompetitionType)
               .Set("SportStats.DistanceType", archery.Distance)
               .Set("SportStats.Bullseyes", CountBullseyes(athlet.loginPlayer))
               .Set("SportStats.MaxPointsPerRound", MaxPointsPerRound(athlet.loginPlayer))
               .Set("SportStats.AveragePointsPerRound", AveragePointsPerRound(athlet.loginPlayer))
               .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        public async Task<int> CountBullseyes(string loginPlayer)
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("NameDesipline", "Archery")),
                new BsonDocument("$unwind", "$playMoves"),
                new BsonDocument("$match", new BsonDocument
                {
                    { "playMoves.loginPlayer", loginPlayer },
                    { "playMoves.typeMove", (int)TypeMove.Bullseyes }
                }),
                new BsonDocument("$count", "BullsCount")
            };

            var result = await _matchEvents
                .Aggregate<BsonDocument>(pipeline)
                .FirstOrDefaultAsync();

            return result?["BullsCount"].AsInt32 ?? 0;
        }
        public async Task<int> MaxPointsPerRound(string loginPlayer)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument
                {
                    { "NameDesipline", "Archery" },
                    { "points.loginPlayer", loginPlayer }
                }),
                new BsonDocument("$unwind", "$points"),
                new BsonDocument("$match", new BsonDocument("points.loginPlayer", loginPlayer)),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "maxPoints", new BsonDocument("$max", "$points.countPoints") }
                })
            };

            var result = await _matchEvents.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();

            return result != null ? result["maxPoints"].AsInt32 : 0;
        }
        public async Task<double> AveragePointsPerRound(string loginPlayer)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument
                {
                    { "NameDesipline", "Archery" },
                    { "points.loginPlayer", loginPlayer }
                }),
                new BsonDocument("$unwind", "$points"),
                new BsonDocument("$match", new BsonDocument("points.loginPlayer", loginPlayer)),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "avgPoints", new BsonDocument("$avg", "$points.countPoints") }
                })
            };

            var result = await _matchEvents.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();

            return result != null ? result["avgPoints"].ToDouble() : 0.0;
        }
        public async Task<Archery?> GetArcheryByMatchId(long idMatch)
        {
            var filter = Builders<Archery>.Filter.Eq(x => x.idMatch, idMatch);
            return await _matchEvents
                .OfType<Archery>()
                .Find(filter)
                .FirstOrDefaultAsync();
        }
    }
}