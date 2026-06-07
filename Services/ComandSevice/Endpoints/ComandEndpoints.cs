using DB.SportHive.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;

namespace Command.Endpoints
{
    public static class CommandService
    {
        public static void CreateCommandEndpoint(this IEndpointRouteBuilder route)
        {
            var routeTeamGroup = route.MapGroup("/team");

            routeTeamGroup.MapPost("/create-team", async (
                [FromForm] TeamModelDto team,
                [FromServices] ITeamOperateService teamService) =>
            {
                await teamService.CreateTeamAsync(team);

                // IMPORTANT FIX:
                // Previously frontend created a team successfully, but OrganizationTeam was not inserted.
                // Now if LoginOrganization is sent in the same form, backend links team immediately.
                if (!string.IsNullOrWhiteSpace(team.LoginOrganization))
                {
                    await teamService.LinkOrganizationTeam(new OrganizationTeamDto
                    {
                        LoginOrganization = team.LoginOrganization.Trim(),
                        NameTeam = team.NameTeam.Trim()
                    });
                }

                return Results.Ok(new
                {
                    message = string.IsNullOrWhiteSpace(team.LoginOrganization)
                        ? "Команду створено"
                        : "Команду створено і прив'язано до організації",
                    nameTeam = team.NameTeam,
                    loginOrganization = team.LoginOrganization
                });
            })
            .DisableAntiforgery()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            routeTeamGroup.MapPost("/add-athletes", async (
                [FromBody] List<TeamAthleteDto> athleteDtos,
                [FromServices] ITeamOperateService teamService) =>
            {
                await teamService.AddAthletes(athleteDtos);
                return Results.Ok(new { message = "Спортсменів додано" });
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            routeTeamGroup.MapPost("/link-team-organization", async (
                [FromBody] OrganizationTeamDto orgTeam,
                [FromServices] ITeamOperateService teamService) =>
            {
                await teamService.LinkOrganizationTeam(orgTeam);
                return Results.Ok(new { message = "Команду прив'язано до організації" });
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            routeTeamGroup.MapPut("/change-status-athlet", async (
                [FromBody] NewSatatusAthlete newSatatus,
                [FromServices] ITeamOperateService teamService) =>
            {
                await teamService.ChangeStatusAthlete(newSatatus);
                return Results.Ok(new { message = "Статус спортсмена змінено" });
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            routeTeamGroup.MapDelete("/remove-athlet", async (
                [FromBody] NewSatatusAthlete newSatatus,
                [FromServices] ITeamOperateService teamService) =>
            {
                await teamService.RemoveAthlet(newSatatus);
                return Results.Ok(new { message = "Спортсмена видалено з команди" });
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
        }
    }
}
