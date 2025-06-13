using DB.SportHive.Domain;
using StackExchange.Redis;

namespace SportHive.Services.Interfaces
{
    public interface IGetMatchesPlayer
    {
        Task<TeamIndivMatch> GetTeamMatches(string loginUser);
        Task<TeamIndivMatch> GetIndividualMatches(string loginUser);
        Task<List<string>> GetExetrmeMatch(string loginUser);

    }
}