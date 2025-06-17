using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Exceptions;
using SportHive.Services.Interfaces;
using SportHive.Implementations;

namespace SportHive.Implementations
{
    public class SystemSelectionService : ISystemSelectionService
    {
        private readonly AppDbContext _appDbContext;
        private readonly SystemFactory _systemFactory;
        public SystemSelectionService(AppDbContext appDbContext, SystemFactory systemFactory)
        {
            _systemFactory = systemFactory;
            _appDbContext = appDbContext;
        }
        public async Task CreateMatchWithSSystem(Matchs teamComposition)
        {
            var IdEvent = await _appDbContext.Events
                .AsNoTracking()
                .Where(e => e.NameEvent == teamComposition.NameEvent)
                .Select(ev => ev.IdEvent)
                .FirstOrDefaultAsync();
            if (IdEvent == null) throw new NotFoundException("Not Found Event");
        
            
            ICompetitionSystem system = _systemFactory.Create(teamComposition.system);
            if(teamComposition.tour == 1) await system.GenerateFirstRoundAsync(teamComposition,IdEvent);
            
        }
    }
}