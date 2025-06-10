using DB.SportHive.Domain;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class DoubleEliminationSystem : ICompetitionSystem
    {
        private readonly IMatchsGenerator _matchsGenerator;
        private readonly IMongoCollection<TeamIndivGrid> _sytemGrid;
        public DoubleEliminationSystem(IMatchsGenerator matchsGenerator,IMongoDbService mongoDbService)
        {
            _sytemGrid = mongoDbService.GetCollection<TeamIndivGrid>("TeamIndivGrid");
            _matchsGenerator = matchsGenerator;
        }
        public async Task GenerateFirstRoundAsync(Matchs matchs, long IdEvent)
        {
            await _matchsGenerator.GenerateInitialBracketAsync(matchs, IdEvent);
        }

        public Task GenerateNextRoundAsync(Matchs matchs, long IdEvent, List<Matchs> previousMatches)
        {
            throw new NotImplementedException();
        }
    }
}
