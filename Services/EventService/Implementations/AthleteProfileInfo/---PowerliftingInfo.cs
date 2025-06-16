using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class PowerliftingInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public PowerliftingInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var achievement = await GetPowerliftingStatsAsync(athlet.loginPlayer);

            var update = Builders<AthleteProfile>.Update
                .Set("SportStats.BestSquatKg", achievement.BestSquatKg)
                .Set("SportStats.BestBenchPressKg", achievement.BestBenchPressKg)
                .Set("SportStats.BestDeadliftKg", achievement.BestDeadliftKg)
                .Set("SportStats.TotalKg", achievement.TotalKg)
                .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        public async Task<PowerliftingStats> GetPowerliftingStatsAsync(string loginPlayer)
        {
            var matches = await _matchEvents.OfType<WeightliftingMatch>()
                                           .Find(_ => true)
                                           .ToListAsync();

            var allLifts = matches.SelectMany(m => m.liftingsAthlete)
                                  .Where(l => l.loginPlayer == loginPlayer && l.done)
                                  .ToList();

            int bestSquat = allLifts
                .Where(l => l.desipline == WeightDesipline.Squat)
                .OrderByDescending(l => l.Weight)
                .Select(l => (int)l.Weight)
                .FirstOrDefault();

            int bestBench = allLifts
                .Where(l => l.desipline == WeightDesipline.BenchPress)
                .OrderByDescending(l => l.Weight)
                .Select(l => (int)l.Weight)
                .FirstOrDefault();

            int bestDeadlift = allLifts
                .Where(l => l.desipline == WeightDesipline.Deadlift)
                .OrderByDescending(l => l.Weight)
                .Select(l => (int)l.Weight)
                .FirstOrDefault();

            return new PowerliftingStats
            {
                BestSquatKg = bestSquat,
                BestBenchPressKg = bestBench,
                BestDeadliftKg = bestDeadlift
            };
        }

    }
}