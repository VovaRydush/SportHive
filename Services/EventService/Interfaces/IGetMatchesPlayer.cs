using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface IGetMatchesPlayer
    {
        Task<TeamIndivMatch> GetTeamMatches(string loginUser);
        Task GetStruggleMatch(string loginUser);
        Task GetCortMatchesMatch(string loginUser);
        Task GetChessMatch(string loginUser);
        Task GetCheckersGame(string loginUser);
        Task GetBoxMatches(string loginUser);
    }
}