using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface IInitalSystemGrid
    {
       Task InitalSystemGrids(long IdEvent,string NameFirstEntity,string NameSecondEntity, Matchs matchs);
    }
}