using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SaveExtremeMatch : ISaveMatchInfo
    {
        private readonly AppDbContext _appDbContext;
        private readonly IEnterDataMatches _dataMatches;
        public SaveExtremeMatch(AppDbContext appDbContext,IEnterDataMatches dataMatches)
        {
            _dataMatches = dataMatches;
            _appDbContext = appDbContext;
        }
        public void SaveMatch(Matchs matchs, long IdExtremeMatches, string TypeMatch)
        {
            var Entitys = new List<EntityInfo>();
            if (TypeMatch == "TeamMatch")
            {
                var entities = new List<EMatchesTeam>();
                foreach (var team in matchs.Entitys)
                {
                    var entity = new EMatchesTeam
                    {
                        IdExtremeMatches = IdExtremeMatches,
                        NameTeam = team.ToString()
                    };
                    Entitys.Add(new EntityInfo { EntityName = team.ToString()});
                    entities.Add(entity);
                }
                _dataMatches.SaveMatches(new ExtremeIndividualInfo
                {
                    idMatch = IdExtremeMatches,
                    tour = matchs.tour,
                    NameDesipline = matchs.NameSport,
                    entitysName = Entitys
                });
                _appDbContext.EMatchesTeam.AddRange(entities);
            }
            else
            {
                var entities = new List<EMatchesAthlete>();
                foreach (var athlete in matchs.Entitys)
                {
                    var entity = new EMatchesAthlete
                    {
                        IdExtremeMatches = IdExtremeMatches,
                        loginAthlete = athlete.ToString()
                    };
                    Entitys.Add(new EntityInfo { EntityName = athlete.ToString()});
                    entities.Add(entity);
                }
                _dataMatches.SaveMatches(new ExtremeIndividualInfo
                {
                    idMatch = IdExtremeMatches,
                    tour = matchs.tour,
                    NameDesipline = matchs.NameSport,
                    entitysName = Entitys
                });
                _appDbContext.ExtremeMatchesAthetes.AddRange(entities);
            }
        }
    }
}