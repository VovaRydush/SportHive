using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SaveExtremeMatch : ISaveMatchInfo
    {
        private readonly AppDbContext _appDbContext;
        public SaveExtremeMatch(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public void SaveMatch(Matchs matchs,long IdExtremeMatches)
        {
            var entities = new List<EMatchesTeam>();
             foreach (var team in matchs.Entitys)
             {
                var entity = new EMatchesTeam
                {
                    IdExtremeMatches = IdExtremeMatches,
                    NameTeam = team.ToString()
                };
                entities.Add(entity);
             }
             _appDbContext.EMatchesTeam.AddRange(entities);
        }
    }
}