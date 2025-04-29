using DB.SportHive.Domain;
namespace SportHive.Services.Interfaces
{
    public interface ICompetitionSystem
    {
        Task GenerateFirstRoundAsync(Matchs matchs, long IdEvent);
        Task GenerateNextRoundAsync(Matchs matchs,long IdEvent, List<Matchs> previousMatches);
    }
}