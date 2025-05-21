
using DB.SportHive.Domain;
using DB.SportHive.Persistence;

namespace SportHive.Services.Interfaces
{
    public interface ISaveMatchInfo
    {
        Task SaveMatch(AppDbContext appDbContext,Matchs matchs, long IdExtremeMatches, string TypeMatch);
        Task SaveMatch(AppDbContext appDbContext,Matchs matchs, string entity1, string entity2, long IdEvent);
    }
}