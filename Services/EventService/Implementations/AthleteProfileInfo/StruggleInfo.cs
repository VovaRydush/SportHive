using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class StruggleInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public StruggleInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var stats = await GetStruggleStatsAsync(athlet.loginPlayer);
            var update = Builders<AthleteProfile>.Update
                .Set("SportStats.Matches", _userUps.CountMatchs(athlet.loginPlayer))
                .Set("SportStats.Wins", _userUps.CountWinIndividual(athlet.loginPlayer))
                .Set("SportStats.Losses", _userUps.CountLossIndividual(athlet.loginPlayer))
                .Set("SportStats.TechnicalWins", stats.TechnicalWins)
                .Set("SportStats.PinWins", stats.PinWins)
                .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        public async Task<StruggleStats> GetStruggleStatsAsync(string login)
        {
            var matches = await _matchEvents.OfType<Struggle>()
                                           .Find(m => m.winner.loginPlayer == login)
                                           .ToListAsync();

            var stats = new StruggleStats
            {
                TechnicalWins = matches.Count(m =>
                    m.winner.win == Result.TechnicalSuperiority ||
                    m.winner.win == Result.Points ||
                    m.winner.win == Result.UnanimousDecision ||
                    m.winner.win == Result.SplitDecision
                ),
                PinWins = matches.Count(m => m.winner.win == Result.Fall)
            };
            
            return stats;
        }
    }
}