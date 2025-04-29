
using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface ISaveMatchInfo
    {
        Task SaveTeamMatch(MatchsAbstractionDto matchs,string Team1, string Team2,long IdEvent);
        Task SaveExtremeMatchTeam(MatchsAbstractionDto matchs,long IdExtremeMatches);
        Task SaveExtremeMatchIndividual(MatchsAbstractionDto matchs);
        Task SaveIndividualMatch(MatchsAbstractionDto matchs, string athlete1, string athlete2,long IdEvent);
    }
}