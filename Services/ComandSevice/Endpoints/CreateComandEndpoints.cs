using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Command.Endpoints
{
    public static class CommandService
    {
        public static void CreateCommandEndpoint(this IEndpointRouteBuilder route)
        {  
            route.MapPost("/create-team", async([FromBody] TeamModelDto team, [FromServices] ITeamOperateService teamService)=>{
                await teamService.CreateTeamAsync(team);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
            
             route.MapPost("/add-athletes", async([FromBody] List<TeamAthleteDto> athleteDtos, [FromServices] ITeamOperateService teamService)=>{
                await teamService.AddAthletes(athleteDtos);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
            
            route.MapPost("/link-team-organization", async([FromBody] OrganizationTeamDto orgTeam, [FromServices] ITeamOperateService teamService)=>{
                await teamService.LinkOrganizationTeam(orgTeam);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
        }
    }
}