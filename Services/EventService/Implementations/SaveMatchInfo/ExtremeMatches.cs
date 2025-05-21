using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SaveExtremeMatch : ISaveMatchInfo
    {
        private readonly IEnterDataMatches _dataMatches;
        public SaveExtremeMatch(IEnterDataMatches dataMatches)
        {
            _dataMatches = dataMatches;
        }
        public async Task SaveMatch(AppDbContext appDbContext,Matchs matchs, long IdExtremeMatches, string TypeMatch)
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
                await _dataMatches.SaveMatches(new ExtremeIndividualInfo
                {
                    idMatch = IdExtremeMatches,
                    tour = matchs.tour,
                    NameDesipline = matchs.NameSport,
                    entitysName = Entitys
                });
                appDbContext.EMatchesTeam.AddRange(entities);
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
                await _dataMatches.SaveMatches(new ExtremeIndividualInfo
                {
                    idMatch = IdExtremeMatches,
                    tour = matchs.tour,
                    NameDesipline = matchs.NameSport,
                    entitysName = Entitys
                });
                appDbContext.ExtremeMatchesAthetes.AddRange(entities);
            }
        }

        public Task SaveMatch(AppDbContext appDbContext,Matchs matchs, string entity1, string entity2, long IdEvent)
        {
            throw new NotImplementedException();
        }
    }
}