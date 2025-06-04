using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class RugbyInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public RugbyInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var playMoves = await GetRugbyStatsAsync(athlet.loginPlayer);
            var update = Builders<AthleteProfile>.Update
                .Set("SportStats.Matches", _userUps.CountMatchs(athlet.loginPlayer))
                .Set("SportStats.Wins", _userUps.CountWinTeam(athlet))
                .Set("SportStats.Losses", _userUps.CountLossTeam(athlet))
                .Set("SportStats.Tries", playMoves.Tries)
                .Set("SportStats.Tackles", playMoves.Tackles)
                .Set("SportStats.PointsScored", playMoves.PointsScored)
                .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        public async Task<RugbyStats> GetRugbyStatsAsync(string login)
        {
            var matches = await _matchEvents.OfType<TeamDesiplines>()
                                           .Find(m => m.playMoves.Any(p => p.loginPlayer == login))
                                           .ToListAsync();

            var playerMoves = matches
                .SelectMany(m => m.playMoves)
                .Where(p => p.loginPlayer == login)
                .ToList();

            var stats = new RugbyStats
            {
                Tries = playerMoves.Count(p => p.typeMove == TypeMove.Tries),
                Tackles = playerMoves.Count(p => p.typeMove == TypeMove.Tackles),
                PointsScored =
                    playerMoves.Count(p => p.typeMove == TypeMove.Tries) * 5 +
                    playerMoves.Count(p => p.typeMove == TypeMove.DropGoal) * 3 +
                    playerMoves.Count(p => p.typeMove == TypeMove.Goal) * 2 +
                    playerMoves.Count(p => p.typeMove == TypeMove.Safety) * 2
            };

            return stats;
        }

    }
}