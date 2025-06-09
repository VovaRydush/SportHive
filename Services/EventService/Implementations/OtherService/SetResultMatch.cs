using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SetResultMatch : ISetResultMatch
    {
        private readonly AppDbContext _appDbContext;
        private readonly IGetPointMatch _getPointMatch;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        private readonly IMongoCollection<TeamIndivGrid> _sytemGrid;
        private readonly AthleteProfileFactory _athleteProfileFactory;
        public SetResultMatch(AppDbContext appDbContext, IMongoDbService mongoDbService, IGetPointMatch getPointMatch, AthleteProfileFactory athleteProfileFactory)
        {
            _athleteProfileFactory = athleteProfileFactory;
            _appDbContext = appDbContext;
            _getPointMatch = getPointMatch;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _sytemGrid = mongoDbService.GetCollection<TeamIndivGrid>("TeamIndivGrid");
        }
        public async Task ChangeStatusMatch(string typeMatch, long idMatch)
        {
            if (typeMatch == "TeamMatch")
            {
                await _appDbContext.TeamMatches.Where(x => x.IdTeamMatch == idMatch).ExecuteUpdateAsync(s => s.SetProperty(c => c.StatusMatch, StatusMatch.Finished));
            }
            if (typeMatch == "IndividualMatch")
            {
                await _appDbContext.IndividualMatches.Where(x => x.IdIndividualMatch == idMatch).ExecuteUpdateAsync(s => s.SetProperty(c => c.StatusMatch, StatusMatch.Finished));
            }
            if (typeMatch == "ExtremeMatch")
            {
                await _appDbContext.ExtremeMatches.Where(x => x.IdExtremeMatches == idMatch).ExecuteUpdateAsync(s => s.SetProperty(c => c.StatusMatch, StatusMatch.Finished));
            }
            await _appDbContext.SaveChangesAsync();
        }
        public async Task SetQualificationGrid(long idMatch, string NameWinner, bool isNext)
        {
            var filter = Builders<TeamIndivGrid>.Filter.And(
                Builders<TeamIndivGrid>.Filter.Eq(x => x.idMatch, idMatch)
            );
            var update = Builders<TeamIndivGrid>.Update.Combine(
                Builders<TeamIndivGrid>.Update.Set("isNext", isNext),
                Builders<TeamIndivGrid>.Update.Set("NameEntity", NameWinner)
            );
            await _sytemGrid.UpdateOneAsync(filter, update);
        }

        public async Task SetResultMatches(long idMatch, string NameWinner, int ScoreEntity1, int ScoreEntity2, int tour)
        {
            var filter = Builders<TeamIndivGrid>.Filter.And(
                Builders<TeamIndivGrid>.Filter.Eq(x => x.idMatch, idMatch)
            );
            var update = Builders<TeamIndivGrid>.Update.Combine(
                Builders<TeamIndivGrid>.Update.Set("NameWinner", NameWinner),
                Builders<TeamIndivGrid>.Update.Set("totalScoreEntity1", ScoreEntity1),
                Builders<TeamIndivGrid>.Update.Set("totalScoreEntity2", ScoreEntity2),
                Builders<TeamIndivGrid>.Update.Set("played", true)
            );
            await _sytemGrid.UpdateOneAsync(filter, update);
        }

        public async Task SetWinnerInMatch(WinnerDto winner)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq(x => x.idMatch, winner.idMatch)
            );
            var update = Builders<MatchEvents>.Update.Combine(
                Builders<MatchEvents>.Update.Set("winner", winner.FullNamePlayer)
            );
            await _matchEvents.UpdateOneAsync(filter, update);
        }

        public async Task SetWinnerInMatchBoard(WinnerDto winner)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq(x => x.idMatch, winner.idMatch)
            );
            var update = Builders<MatchEvents>.Update.Combine(
                Builders<MatchEvents>.Update.Set("winner", new BoardWinner
                {
                    FullNamePlayer = winner.FullNamePlayer,
                    loginPlayer = winner.loginWinner,
                    typeWin = winner.win,
                    countPoints = winner.countPoints ?? -1,
                })
            );
            await _matchEvents.UpdateOneAsync(filter, update);
        }

        public async Task SetWinnerInMatchStruggle(WinnerDto winner)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                Builders<MatchEvents>.Filter.Eq(x => x.idMatch, winner.idMatch)
            );
            EnumWork.TryParseStyleFromText<Result>(winner.win, out int typeMoves);
            var update = Builders<MatchEvents>.Update.Combine(
                Builders<MatchEvents>.Update.Set("winner", new WinStruggleResult
                {
                    FullNamePlayer = winner.FullNamePlayer,
                    loginPlayer = winner.loginWinner,
                    win = (Result)typeMoves,
                    countPoints = Convert.ToInt16(winner.countPoints),
                })
            );
            await _matchEvents.UpdateOneAsync(filter, update);
        }
    }
}