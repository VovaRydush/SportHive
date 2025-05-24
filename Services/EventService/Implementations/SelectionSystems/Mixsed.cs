using DB.SportHive.Domain;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class MixsedSystem : ICompetitionSystem
    {
        private readonly SaveMatchFactory _saveMatchFactory;
        private readonly GroupSystem _groupSystem;
        public MixsedSystem(SaveMatchFactory saveMatchFactory)
        {
            _saveMatchFactory = saveMatchFactory;
        }
        public async Task GenerateFirstRoundAsync(Matchs matchs, long IdEvent)
        {
            await _groupSystem.GenerateFirstRoundAsync(matchs, IdEvent);
        }

        public Task GenerateNextRoundAsync(Matchs matchs, long IdEvent, List<Matchs> previousMatches)
        {
            throw new NotImplementedException();
        }
    }
}
