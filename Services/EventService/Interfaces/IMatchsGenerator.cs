using DB.SportHive.Domain;
namespace SportHive.Services.Interfaces
{
    public interface IMatchsGenerator
    {
       Task GenerateInitialBracketAsync(Matchs matchs, long IdEvent);
    }
}