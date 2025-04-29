using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SaveExtremeMatchIndividual : ISaveMatchInfo
    {
        private readonly AppDbContext _appDbContext;
        public SaveExtremeMatchIndividual(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public void SaveMatch()
        {
           
        }
    }
}