using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class CheckersChessInfo : IProfileInfo
    {
        private readonly INuclearRap _userUps;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public CheckersChessInfo(INuclearRap userUps, IMongoDbService mongoDbService)
        {
            _userUps = userUps;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task SportInfo(AthletesTeamDto athlet)
        {
            var update = Builders<AthleteProfile>.Update
               .Set("SportStats.Matches", _userUps.CountMatchs(athlet.loginPlayer))
               .Set("SportStats.Losses", _userUps.CountLossIndividual(athlet.loginPlayer))
               .Set("SportStats.Wins", _userUps.CountWinIndividual(athlet.loginPlayer))
               .Set("SportStats.Draws", _userUps.CountDrawIndividual(athlet.loginPlayer))
               .Set("SportStats.TournamentMedals", GetTournamentMedalsAsync(athlet.loginPlayer))
               .Set("SportStats.FastestWinMoves", GetFastestWinMovesAsync(athlet.loginPlayer))
               .Set("SportStats.AverageMoveTimeSeconds", GetAverageMoveTimeSecondsAsync(athlet.loginPlayer))
               .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, athlet.loginPlayer),
                update: update
            );
        }
        public async Task<int> GetFastestWinMovesAsync(string loginPlayer)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq("_t", nameof(ChessMatch)),
                Builders<MatchEvents>.Filter.Eq("Result.loginPlayer", loginPlayer)
            );

            var projection = Builders<MatchEvents>.Projection.Include("Moves");

            var matches = await _matchEvents.Find(filter).Project<ChessMatch>(projection).ToListAsync();

            return matches
                .Where(m => m.winner?.loginPlayer == loginPlayer)
                .Select(m => m.Moves?.Count ?? int.MaxValue)
                .DefaultIfEmpty(int.MaxValue)
                .Min();
        }

        public async Task<double> GetAverageMoveTimeSecondsAsync(string loginPlayer)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq("_t", nameof(ChessMatch)), // або nameof(CheckersMatch)
                Builders<MatchEvents>.Filter.Or(
                    Builders<MatchEvents>.Filter.Eq("FullNamePlayer1", loginPlayer),
                    Builders<MatchEvents>.Filter.Eq("FullNamePlayer2", loginPlayer)
                )
            );

            var projection = Builders<MatchEvents>.Projection.Include("Moves");

            var matches = await _matchEvents.Find(filter).Project<ChessMatch>(projection).ToListAsync();

            var allTimes = new List<TimeSpan>();

            foreach (var match in matches)
            {
                var moves = match.Moves.Where(m => m.Player == loginPlayer).ToList();

                for (int i = 1; i < moves.Count; i++)
                {
                    var prev = moves[i - 1];
                    var curr = moves[i];

                    var timeUsed = (curr.TimeRemainingWhite ?? TimeSpan.Zero) - (prev.TimeRemainingWhite ?? TimeSpan.Zero);
                    allTimes.Add(timeUsed.Duration());
                }
            }

            return allTimes.Count > 0
                ? allTimes.Average(t => t.TotalSeconds)
                : 0;
        }

        public async Task<List<string>> GetTournamentMedalsAsync(string loginPlayer)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq("_t", nameof(ChessMatch)), // або nameof(CheckersMatch)
                Builders<MatchEvents>.Filter.Eq("Result.loginPlayer", loginPlayer)
            );

            var projection = Builders<MatchEvents>.Projection.Include("Result.typeWin");

            var matches = await _matchEvents.Find(filter).Project<BsonDocument>(projection).ToListAsync();

            return matches
                .Select(m => m["Result"]["typeWin"].AsString)
                .Where(win => win == "Gold" || win == "Silver" || win == "Bronze") // або інші критерії
                .ToList();
        }


    }
}