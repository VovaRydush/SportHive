using DB.SportHive.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;

namespace Events.Endpoints
{
    public static class Stage3TournamentEndpoints
    {
        public static void Stage3TournamentEndpoint(this IEndpointRouteBuilder route)
        {
            var group = route.MapGroup("/stage3");

            group.MapPost("/events", async ([FromBody] Stage3CreateEventDto dto, [FromServices] IStage3TournamentService service) =>
            {
                return Results.Ok(await service.CreateEventAsync(dto));
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            group.MapPost("/events/generate", async ([FromBody] Stage3GenerateMatchesDto dto, [FromServices] IStage3TournamentService service) =>
            {
                return Results.Ok(await service.GenerateMatchesAsync(dto));
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            group.MapGet("/events/{eventName}", async (string eventName, [FromServices] IStage3TournamentService service) =>
            {
                return Results.Ok(await service.GetWorkspaceAsync(eventName));
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization,Athlete,Judge" });

            group.MapGet("/matches/{matchType}/{idMatch:long}", async (string matchType, long idMatch, [FromServices] IStage3TournamentService service) =>
            {
                return Results.Ok(await service.GetMatchAsync(matchType, idMatch));
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization,Athlete,Judge" });

            group.MapPatch("/matches/result", async ([FromBody] Stage3SetMatchResultDto dto, [FromServices] IStage3TournamentService service) =>
            {
                return Results.Ok(await service.SetResultAsync(dto));
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization,Judge" });

            group.MapPatch("/matches/status", async ([FromBody] Stage3ChangeMatchStatusDto dto, [FromServices] IStage3TournamentService service) =>
            {
                return Results.Ok(await service.ChangeStatusAsync(dto));
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization,Judge" });
        }
    }
}
