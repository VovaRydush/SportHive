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
        public async Task SetResultMatches(string nameWinner, string resultWinner, long idMatch, string typeMatch)
        {
            await ChangeStatusMatch(typeMatch, idMatch);

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
        public async Task SetResultsInGrid()
        {
            await Task.CompletedTask;
        }
    }
}