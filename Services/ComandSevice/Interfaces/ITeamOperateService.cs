using  DB.SportHive.Domain;
namespace SportHive.Services.Interfaces
{
 public interface ITeamOperateService
 { 
    Task CreateTeamAsync(TeamModelDto team);
    Task AddAthletes(List<TeamAthleteDto> athleteDto);
    Task ChangeStatusAthlete(NewSatatusAthlete newSatatus);
    Task LinkOrganizationTeam(OrganizationTeamDto dto);
    Task RemoveAthlet(NewSatatusAthlete newSatatus);
    Task<TeamAthlete> GetAthlete(string loginAthlets,string NameTeam);
 }
}