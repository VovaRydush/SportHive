using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SwissSystem : ICompetitionSystem
    {
        private readonly IMatchsGenerator _matchsGenerator;
        public SwissSystem(IMatchsGenerator matchsGenerator)
        {
            _matchsGenerator = matchsGenerator;
        }
        public async Task GenerateFirstRoundAsync(Matchs matchs, long IdEvent)
        {
            if (matchs.Rating == false) await GenerateFirstRoundRandomAsync(matchs, IdEvent);
            else await GenerateFirstRoundByRatingAsync(matchs,IdEvent);
        }

        public Task GenerateNextRoundAsync(Matchs matchs, long IdEvent, List<Matchs> previousMatches)
        {
            throw new NotImplementedException();
        }
        
        public async Task GenerateFirstRoundRandomAsync(Matchs matchs, long IdEvent)
        {
            await _matchsGenerator.GenerateInitialBracketAsync(matchs,IdEvent);
        }
        public async Task GenerateFirstRoundByRatingAsync(Matchs matchs, long IdEvent)
        {
            await Task.CompletedTask;
        }
    }
}
