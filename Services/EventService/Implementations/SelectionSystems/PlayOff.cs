using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class PlayOffSystem : ICompetitionSystem
    {
       
        private readonly IMatchsGenerator _matchsGenerator;
        public PlayOffSystem(IMatchsGenerator matchsGenerator)
        {
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