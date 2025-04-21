using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface IGetInfoTeam
    {
       Task<List<AthleteTeamDto>> GetAthetesAsync(string NameTeam);
       Task GetTeam();
    }
}
