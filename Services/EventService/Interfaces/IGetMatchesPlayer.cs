using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface IGetMatchesPlayer
    {
        Task<TeamIndivMatch> GetTeamMatches(string loginUser);
        Task<TeamIndivMatch> GetIndividualMatches(string loginUser);
        // бокс шашки шахмати бадмінтон теніс настільний теніс бродьба
    }
}