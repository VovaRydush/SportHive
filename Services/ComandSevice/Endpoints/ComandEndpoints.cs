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
            var routeTeamGroup = route.MapGroup("/team");

            routeTeamGroup.MapPost("/create-team", async ([FromForm] TeamModelDto team, [FromServices] ITeamOperateService teamService) =>
            {
                await teamService.CreateTeamAsync(team);
            }).DisableAntiforgery() 
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            routeTeamGroup.MapPost("/add-athletes", async ([FromBody] List<TeamAthleteDto> athleteDtos, [FromServices] ITeamOperateService teamService) =>
            {
                await teamService.AddAthletes(athleteDtos);
            })
               .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            routeTeamGroup.MapPost("/link-team-organization", async ([FromBody] OrganizationTeamDto orgTeam, [FromServices] ITeamOperateService teamService) =>
            {
                await teamService.LinkOrganizationTeam(orgTeam);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            routeTeamGroup.MapPut("/change-status-athlet", async ([FromBody] NewSatatusAthlete newSatatus, [FromServices] ITeamOperateService teamService) =>
            {
                await teamService.ChangeStatusAthlete(newSatatus);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            routeTeamGroup.MapDelete("/remove-athlet", async ([FromBody] NewSatatusAthlete newSatatus, [FromServices] ITeamOperateService teamService) =>
            {
                await teamService.RemoveAthlet(newSatatus);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
        }
    }
}