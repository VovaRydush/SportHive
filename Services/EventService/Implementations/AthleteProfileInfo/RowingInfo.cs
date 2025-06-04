using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class RowingInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public RowingInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var bestResults = await GetRowingStatsAsync(athlet.loginPlayer);
            var update = Builders<AthleteProfile>.Update
                .Set("SportStats.Matches", _userUps.CountMatchs(athlet.loginPlayer))
                .Set("SportStats.BestTimeSeconds", bestResults.BestTimeSeconds)
                .Set("SportStats.BestDistance", bestResults.BestDistance)
                .Set("SportStats.BoatType", GetMostCommonBoatTypeAsync(athlet.loginPlayer))
                .Set("SportStats.Discipline",GetMostCommonDisciplineAsync(athlet.loginPlayer))
                .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
            throw new NotImplementedException();
        }
        public async Task<RowingStats> GetRowingStatsAsync(string athleteOrTeam)
        {
            var matches = await _matchEvents.OfType<Rowing>()
                                           .Find(_ => true)
                                           .ToListAsync();

            var allResults = matches.SelectMany(m => m.rowingAtheletes)
                                    .Where(r => r.Finished && r.AthleteOrTeam == athleteOrTeam)
                                    .ToList();

            var best = allResults
                .OrderBy(r => r.ResultTime)
                .FirstOrDefault();

            if (best == null)
                return new RowingStats { BestTimeSeconds = 0, BestDistance = "N/A" };

            return new RowingStats
            {
                BestTimeSeconds = best.ResultTime.TotalSeconds,
                BestDistance = $"{best.DistanceMeters}m"
            };
        }

        public async Task<string?> GetMostCommonBoatTypeAsync(string athleteOrTeam)
        {
            var matches = await _matchEvents.OfType<Rowing>()
                                           .Find(_ => true)
                                           .ToListAsync();

            var mostCommon = matches
                .Where(m => m.rowingAtheletes.Any(r => r.AthleteOrTeam == athleteOrTeam))
                .GroupBy(m => m.BoatType)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            return mostCommon;
        }
        public async Task<string?> GetMostCommonDisciplineAsync(string athleteOrTeam)
        {
            var matches = await _matchEvents.OfType<Rowing>()
                                           .Find(_ => true)
                                           .ToListAsync();

            var mostCommon = matches
                .Where(m => m.rowingAtheletes.Any(r => r.AthleteOrTeam == athleteOrTeam))
                .GroupBy(m => m.Discipline)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            return mostCommon;
        }
    }
}