using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class BoxInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public BoxInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var update = Builders<AthleteProfile>.Update
               .Set("SportStats.Matches", _userUps.CountMatchs(athlet.loginPlayer))
               .Set("SportStats.Losses", _userUps.CountLossIndividual(athlet.loginPlayer))
               .Set("SportStats.Wins", _userUps.CountWinIndividual(athlet.loginPlayer))
               .Set("SportStats.Knockouts", GetKnockoutWinsAsync(athlet.loginPlayer))
               .Set("SportStats.RoundsFought", GetRoundsFoughtAsync(athlet.loginPlayer))
               .Set("SportStats.AverageScorePerRound", GetAverageScorePerRoundAsync(athlet.loginPlayer))
               .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        public async Task<int> GetKnockoutWinsAsync(string loginPlayer)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq("_t", nameof(Box)),
                Builders<MatchEvents>.Filter.Eq("winner.loginPlayer", loginPlayer),
                Builders<MatchEvents>.Filter.In("winner.win", new[] { Result.Knockout, Result.TechnicalKnockout })
            );

            var count = await _matchEvents.CountDocumentsAsync(filter);
            return (int)count;
        }

        public async Task<int> GetRoundsFoughtAsync(string loginPlayer)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq("_t", nameof(Box)),
                Builders<MatchEvents>.Filter.Or(
                    Builders<MatchEvents>.Filter.Eq("FullNamePlayer1", loginPlayer),
                    Builders<MatchEvents>.Filter.Eq("FullNamePlayer2", loginPlayer),
                    Builders<MatchEvents>.Filter.ElemMatch<BsonDocument>(
                        "points", 
                        Builders<BsonDocument>.Filter.Eq("loginPlayer", loginPlayer)
                    )
                )
            );

            var matches = await _matchEvents.Find(filter).ToListAsync();
            var boxingMatches = matches.OfType<Box>();

            return boxingMatches
                .SelectMany(m => m.points)
                .Count(p => p.loginPlayer == loginPlayer);
        }


        public async Task<double> GetAverageScorePerRoundAsync(string loginPlayer)
        {
            var filter = Builders<MatchEvents>.Filter.Eq("_t", nameof(Box));
            var matches = await _matchEvents.Find(filter).ToListAsync();
            var boxingMatches = matches.Cast<Box>();

            var relevantPoints = boxingMatches
                .SelectMany(m => m.points)
                .Where(p => p.loginPlayer == loginPlayer)
                .ToList();

            if (!relevantPoints.Any())
                return 0;

            var totalPoints = relevantPoints.Sum(p => p.countPoints);
            return (double)totalPoints / relevantPoints.Count;
        }
    }
}