using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class WeightliftingInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public WeightliftingInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var weightlifting = await GetPowerliftingStatsAsync(athlet.loginPlayer);
            var update = Builders<AthleteProfile>.Update
                .Set("SportStats.BestSnatchKg", weightlifting.BestSnatchKg)
                .Set("SportStats.BestCleanAndJerkKg", weightlifting.BestCleanAndJerkKg)
                .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        public async Task<WeightliftingStats> GetPowerliftingStatsAsync(string loginPlayer)
        {
            var matches = await _matchEvents.OfType<WeightliftingMatch>()
                                           .Find(_ => true)
                                           .ToListAsync();

            var allLifts = matches.SelectMany(m => m.liftingsAthlete)
                                  .Where(l => l.loginPlayer == loginPlayer && l.done)
                                  .ToList();

            int bestSnatch = allLifts
                .Where(l => l.desipline == WeightDesipline.Snatch)
                .OrderByDescending(l => l.Weight)
                .Select(l => (int)l.Weight)
                .FirstOrDefault();

            int BestCleanAndJerkKg = allLifts
                .Where(l => l.desipline == WeightDesipline.CleanJerk)
                .OrderByDescending(l => l.Weight)
                .Select(l => (int)l.Weight)
                .FirstOrDefault();

            return new WeightliftingStats
            {
                BestSnatchKg = bestSnatch,
                BestCleanAndJerkKg = BestCleanAndJerkKg,
            };
        }
    }
}