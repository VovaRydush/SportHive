using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class BasketballInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public BasketballInfo()
        { }
        public BasketballInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var update = Builders<AthleteProfile>.Update
               .Set("SportStats.Matches", _userUps.CountMatchs(athlet.loginPlayer))
               .Set("SportStats.Losses", _userUps.CountLossTeam(athlet))
               .Set("SportStats.Wins", _userUps.CountWinTeam(athlet))
               .Set("SportStats.Points", GetPointsAsync(athlet.loginPlayer))
               .Set("SportStats.Rebounds", GetReboundsAsync(athlet.loginPlayer))
               .Set("SportStats.Assists", GetAssistsAsync(athlet.loginPlayer))
               .Set("SportStats.Blocks", GetBlocksAsync(athlet.loginPlayer))
               .Set("SportStats.ThreePointPercentage", GetThreePointPercentageAsync(athlet.loginPlayer))
               .Set("SportStats.FreeThrowPercentage",GetFreeThrowPercentageAsync(athlet.loginPlayer))
               .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        private async Task<List<TeamDesiplines>> GetAllTeamDesiplinesMatchesAsync()
        {
            var all = await _matchEvents.Find(x => x is TeamDesiplines).ToListAsync();
            return all.OfType<TeamDesiplines>().ToList();
        }
        public async Task<int> GetPointsAsync(string login)
        {
            var matches = await GetAllTeamDesiplinesMatchesAsync();
            return matches
                .SelectMany(m => m.playMoves)
                .Count(m => m.loginPlayer == login && m.typeMove == TypeMove.Goal);
        }

        public async Task<int> GetReboundsAsync(string login)
        {
            var matches = await GetAllTeamDesiplinesMatchesAsync();
            return matches
                .SelectMany(m => m.playMoves)
                .Count(m => m.loginPlayer == login &&
                            (m.typeMove == TypeMove.DefensiveRebound || m.typeMove == TypeMove.OffensiveRebound));
        }

        public async Task<int> GetAssistsAsync(string login)
        {
            var matches = await GetAllTeamDesiplinesMatchesAsync();
            return matches
                .SelectMany(m => m.twoPlayersMoves)
                .Count(m => m.FullNamePlayer == login);
        }

        public async Task<int> GetBlocksAsync(string login)
        {
            var matches = await GetAllTeamDesiplinesMatchesAsync();
            return matches
                .SelectMany(m => m.attacksMoves)
                .Count(m => m.FullNamePlayer == login && m.move == TypeMoves.Block);
        }

        public async Task<double> GetThreePointPercentageAsync(string login)
        {
            var matches = await GetAllTeamDesiplinesMatchesAsync();
            var all = matches
                .SelectMany(m => m.attacksMoves)
                .Where(m => m.FullNamePlayer == login && m.move == TypeMoves.ShotOnGoal);

            int total = all.Count();
            int success = all.Count(m => m.realization);

            return total == 0 ? 0 : Math.Round((double)success / total * 100, 2);
        }

        public async Task<double> GetFreeThrowPercentageAsync(string login)
        {
            var matches = await GetAllTeamDesiplinesMatchesAsync();
            var all = matches
                .SelectMany(m => m.attacksMoves)
                .Where(m => m.FullNamePlayer == login && m.move == TypeMoves.Penalty);

            int total = all.Count();
            int success = all.Count(m => m.realization);

            return total == 0 ? 0 : Math.Round((double)success / total * 100, 2);
        }
    }
}