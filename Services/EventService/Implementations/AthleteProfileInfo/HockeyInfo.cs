using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class HockeyInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        private readonly BasketballInfo basketball;
        public HockeyInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            basketball = new BasketballInfo();
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var update = Builders<AthleteProfile>.Update
                .Set("SportStats.Matches", _userUps.CountMatchs(athlet.loginPlayer))
                .Set("SportStats.Goals", GetHockeyGoalsAsync(athlet.loginPlayer))
                .Set("SportStats.Assists", basketball.GetAssistsAsync(athlet.loginPlayer))
                .Set("SportStats.Wins", _userUps.CountWinTeam(athlet))
                .Set("SportStats.Losses", _userUps.CountLossTeam(athlet))
                .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        public async Task<int> GetHockeyGoalsAsync(string loginPlayer)
        {
            var filter = Builders<TeamDesiplines>.Filter.Eq(td => td.NameDesipline, "Hockey");
            var documents = await _matchEvents.OfType<TeamDesiplines>()
                                             .Find(filter)
                                             .ToListAsync();

            int totalGoals = 0;
            foreach (var doc in documents)
            {
                totalGoals += doc.playMoves.Count(pm => pm.loginPlayer == loginPlayer && pm.typeMove == TypeMove.Goal);
            }
            return totalGoals;
        }
    }
}