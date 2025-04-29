using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.VisualBasic;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class RoundRobinSystem : ICompetitionSystem
    {
        private readonly SaveMatchFactory _saveMatchFactory;
        private readonly AppDbContext _appDbContext;
        public RoundRobinSystem(SaveMatchFactory saveMatchFactory,AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _saveMatchFactory = saveMatchFactory;
        }
        public async Task GenerateFirstRoundAsync(Matchs matchs, long IdEvent)
        {
            var saveEntity = _saveMatchFactory.Create(matchs.typeSport);

            for (int i = 0; i < matchs.Entitys.Count - 1; i++)
            {
                for (int j = i + 1; j < matchs.Entitys.Count; j++)
                {
                    saveEntity.SaveMatch(matchs, matchs.Entitys[i], matchs.Entitys[j], IdEvent);
                }
            }
           await _appDbContext.SaveChangesAsync(); 
        }

        public Task GenerateNextRoundAsync(Matchs matchs, long IdEvent, List<Matchs> previousMatches)
        {
            throw new NotImplementedException();
        }
    }
}
