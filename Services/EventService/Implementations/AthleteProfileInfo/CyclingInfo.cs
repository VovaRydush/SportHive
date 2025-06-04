using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class CyclingInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public CyclingInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var update = Builders<AthleteProfile>.Update
                .Set("SportStats.Matches", _userUps.CountMatchs(athlet.loginPlayer))
                .Set("SportStats.BestTime", GetBestTimeAsync(athlet.loginPlayer))
                .Set("SportStats.BestDistance", GetBestDistanceAsync(athlet.loginPlayer))
                .Set("SportStats.AverageSpeedKmH", GetAverageSpeedAsync(athlet.loginPlayer)) 
                .Set("SportStats.Wins", _userUps.CountWinIndividual(athlet.loginPlayer))
                .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        public async Task<double?> GetBestTimeAsync(string loginPlayer)
        {
            var athleteFilter = Builders<CyclingRace>.Filter.And(
                Builders<CyclingRace>.Filter.Eq(r => r.loginPlayer, loginPlayer),
                Builders<CyclingRace>.Filter.Eq(r => r.DidNotFinish, false)
            );

            var mainFilter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq("_t", nameof(Cycling)),
                Builders<MatchEvents>.Filter.ElemMatch("atheltesMoves", athleteFilter)
            );

            var projection = Builders<MatchEvents>.Projection.Include("atheltesMoves");

            var documents = await _matchEvents.Find(mainFilter).Project(projection).ToListAsync();

            double? bestTimeSeconds = null;

            foreach (var doc in documents)
            {
                var cycling = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<Cycling>(doc);

                var athleteMoves = cycling.atheltesMoves
                    .Where(a => a.loginPlayer == loginPlayer && !a.DidNotFinish);

                var minFinishTime = athleteMoves.Min(a => (double?)a.FinishTime.TotalSeconds);

                if (minFinishTime != null && (bestTimeSeconds == null || minFinishTime < bestTimeSeconds))
                {
                    bestTimeSeconds = minFinishTime;
                }
            }

            return bestTimeSeconds;
        }

        // Метод для отримання найкращої дистанції (максимальної DistanceKm)
        public async Task<float?> GetBestDistanceAsync(string loginPlayer)
        {
            var athleteFilter = Builders<CyclingRace>.Filter.And(
                Builders<CyclingRace>.Filter.Eq(r => r.loginPlayer, loginPlayer),
                Builders<CyclingRace>.Filter.Eq(r => r.DidNotFinish, false)
            );

            var mainFilter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq("_t", nameof(Cycling)),
                Builders<MatchEvents>.Filter.ElemMatch("atheltesMoves", athleteFilter)
            );

            var projection = Builders<MatchEvents>.Projection.Include("atheltesMoves");

            var documents = await _matchEvents.Find(mainFilter).Project(projection).ToListAsync();

            float? bestDistance = null;

            foreach (var doc in documents)
            {
                var cycling = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<Cycling>(doc);

                var athleteMoves = cycling.atheltesMoves
                    .Where(a => a.loginPlayer == loginPlayer && !a.DidNotFinish);

                var maxDistance = athleteMoves.Max(a => (float?)a.DistanceKm);

                if (maxDistance != null && (bestDistance == null || maxDistance > bestDistance))
                {
                    bestDistance = maxDistance;
                }
            }

            return bestDistance;
        }

        public async Task<double> GetAverageSpeedAsync(string loginPlayer)
        {
            var raceFilter = Builders<CyclingRace>.Filter.And(
                Builders<CyclingRace>.Filter.Eq(r => r.loginPlayer, loginPlayer),
                Builders<CyclingRace>.Filter.Eq(r => r.DidNotFinish, false)
            );

            var filter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq("_t", nameof(Cycling)),
                Builders<MatchEvents>.Filter.ElemMatch("atheltesMoves", raceFilter)
            );

            var projection = Builders<MatchEvents>.Projection.Include("atheltesMoves");

            var matches = await _matchEvents.Find(filter).Project<Cycling>(projection).ToListAsync();

            var allSpeeds = matches
                .SelectMany(m => m.atheltesMoves)
                .Where(r => r.loginPlayer == loginPlayer && !r.DidNotFinish)
                .Select(r => (double)r.AvgSpeed)
                .ToList();

            return allSpeeds.Count > 0 ? allSpeeds.Average() : 0;
        }
    }
}