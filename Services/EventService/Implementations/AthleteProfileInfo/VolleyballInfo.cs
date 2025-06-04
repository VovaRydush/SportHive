using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class VolleyballInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public VolleyballInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var moves = await GetInfoVolleyballAsync(athlet.loginPlayer);
            var update = Builders<AthleteProfile>.Update
                .Set("SportStats.Matches", _userUps.CountMatchs(athlet.loginPlayer))
                .Set("SportStats.Wins", _userUps.CountWinTeam(athlet))
                .Set("SportStats.Losses", _userUps.CountLossTeam(athlet))
                .Set("SportStats.Blocks", moves.Blocks)
                .Set("SportStats.Errors", moves.Errors)
                .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        public async Task<VolleyballStats> GetInfoVolleyballAsync(string loginPlayer)
        {
            var filter = Builders<TeamDesiplines>.Filter.Eq("NameDesipline", "Volleyball");
            var documents = await _matchEvents.OfType<TeamDesiplines>()
                                             .Find(filter)
                                             .ToListAsync();

            int errors = 0;
            int blocks = 0;
            foreach (var doc in documents)
            {
                errors += doc.fouls.Count(f => f.loginPlayer == loginPlayer && (f.foul == Foul.UnforcedError || f.foul == Foul.ForcedError));
                blocks += doc.attacksMoves.Count(c => c.loginPlayer == loginPlayer && c.move == TypeMoves.Block);
            }
            return new VolleyballStats
            {
                Errors = errors,
                Blocks = blocks 
            };
        }
    }
}