using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SwimmingInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public SwimmingInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var stats = await GetSwimmingStatsAsync(athlet.loginPlayer);
            var update = Builders<AthleteProfile>.Update
                .Push("SportStats.Matches", stats)
                .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        public async Task<SwimmingStats> GetSwimmingStatsAsync(string loginPlayer)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq("_t", nameof(Swimming)),
                Builders<MatchEvents>.Filter.ElemMatch("athleteSwimming",
                    Builders<BsonDocument>.Filter.Eq("loginPlayer", loginPlayer))
            );

            var projection = Builders<MatchEvents>.Projection.Include("athleteSwimming");

            var matches = await _matchEvents
                .Find(filter)
                .Project<Swimming>(projection)
                .ToListAsync();

            var disciplines = new Dictionary<(string StrokeType, int Distance), TimeSpan>();

            foreach (var match in matches)
            {
                foreach (var athlete in match.athleteSwimming)
                {
                    if (athlete.loginPlayer != loginPlayer)
                        continue;

                    var key = (athlete.style.ToString(), athlete.DistanceMeters);

                    if (!disciplines.ContainsKey(key) || athlete.time < disciplines[key])
                    {
                        disciplines[key] = athlete.time;
                    }
                }
            }

            return new SwimmingStats
            {
                Disciplines = disciplines.Select(d => new SwimmingDiscipline
                {
                    StrokeType = d.Key.StrokeType,
                    DistanceMeters = d.Key.Distance,
                    BestTime = d.Value.ToString(@"hh\:mm\:ss\.fff")
                }).ToList()
            };
        }

    }
}