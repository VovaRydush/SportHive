using  DB.SportHive.Domain;
namespace SportHive.Services.Interfaces
{
 public interface ITeamOperateService
 { 
    Task CreateTeamAsync(TeamModelDto team);
 }
}