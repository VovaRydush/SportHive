using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class QualificationByStandard : ICompetitionSystem
    {
        private readonly SaveMatchFactory _saveMatchFactory;
        private readonly AppDbContext _appDbContext;
        public QualificationByStandard(SaveMatchFactory saveMatchFactory, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _saveMatchFactory = saveMatchFactory;
        }
        public async Task GenerateFirstRoundAsync(Matchs matchs, long IdEvent)
        {
            var saveEntity = _saveMatchFactory.Create(matchs.typeSport);
            await saveEntity.SaveMatch(_appDbContext, matchs, IdEvent, matchs.typeSport);
            await _appDbContext.SaveChangesAsync();
        }

        public Task GenerateNextRoundAsync(Matchs matchs, long IdEvent)
        {
            throw new NotImplementedException();
        }
    }
}