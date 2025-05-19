using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class MatchsGenerator : IMatchsGenerator
    {
        private readonly SaveMatchFactory _saveMatchFactory;
        private readonly AppDbContext _appDbContext;
        public MatchsGenerator(SaveMatchFactory saveMatchFactory, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _saveMatchFactory = saveMatchFactory;
        }
        public async Task GenerateInitialBracketAsync(Matchs matchs,long IdEvent)
        {
            var saveEntity = _saveMatchFactory.Create(matchs.typeSport);
            int iterator = 0;
            if (matchs.Entitys.Count % 2 != 0) { saveEntity.SaveMatch(matchs, matchs.Entitys[0], "Bye", IdEvent); iterator++; }
            for (int i = iterator; i < matchs.Entitys.Count - 1; i += 2)
            {
                saveEntity.SaveMatch(matchs, matchs.Entitys[i],matchs.Entitys[i+1], IdEvent);
            }
           await _appDbContext.SaveChangesAsync(); 
        }
    }
}