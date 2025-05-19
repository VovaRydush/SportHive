using DB.SportHive.Domain;
using DB.SportHive.Persistence;
namespace SportHive.Services.Interfaces
{
    public interface IMatchsGenerator
    {
       Task GenerateInitialBracketAsync(Matchs matchs, long IdEvent);
    }
}