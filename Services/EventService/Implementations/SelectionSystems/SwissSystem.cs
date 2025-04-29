using DB.SportHive.Domain;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SwissSystem : ICompetitionSystem
    {
        private readonly SaveMatchFactory _saveMatchFactory;
        public SwissSystem(SaveMatchFactory saveMatchFactory)
        {
            _saveMatchFactory = saveMatchFactory;
        }
        public Task GenerateFirstRoundAsync(Matchs matchs, long IdEvent)
        {
            throw new NotImplementedException();
        }

        public Task GenerateNextRoundAsync(Matchs matchs, long IdEvent, List<Matchs> previousMatches)
        {
            throw new NotImplementedException();
        }
    }
}
