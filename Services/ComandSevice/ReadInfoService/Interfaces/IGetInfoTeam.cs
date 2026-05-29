using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface IGetInfoTeam
    {
        Task<List<AthleteTeamDto>> GetAthetesAsync(string nameTeam);
        Task<TeamInfoDto> GetTeamInfo(string nameTeam);
        Task<List<TeamInfoDto>> GetTeamsByOrganizationAsync(string loginOrganization);
        Task<List<TeamInfoDto>> GetTeamsByTrainerAsync(string loginTrainer);
    }
}
