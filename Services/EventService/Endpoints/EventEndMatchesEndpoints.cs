using DB.SportHive.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using  SportHive.Services.Interfaces;

namespace Events.Endpoints
{
    public static class CreateEventService
    {
        public static void EventsEndpoints(this IEndpointRouteBuilder route)
        {
            route.MapPost("/create-event", async ([FromForm] EventDto Event, [FromServices] IEventService eventService) =>
            {
                await eventService.CreateEvent(Event);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization,Athlete" });

            route.MapPost("/inital-create-matches", async ([FromBody] Matchs match, [FromServices] ISystemSelectionService matchService) =>
            {
                await matchService.CreateMatchWithSSystem(match);
                return Results.Ok();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/add-athletes-em/{IdExtremeMatches}", async (long IdExtremeMatches, [FromBody] List<string> atletes, [FromServices] IEventService eventService) =>
            {
                await eventService.SaveExtreameAtheltes(atletes, IdExtremeMatches);
                return Results.Ok();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
            route.MapPost("/set-attack-moment", async (TwoPlayerMoveDto move, [FromServices] IEnterSportMove enterDataMatches) =>
            {
                await enterDataMatches.SetAttackMoves(move);
                return Results.Ok();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
            route.MapPost("/set-foul", async (PlayerMovesDto move, [FromServices] IEnterSportMove enterDataMatches) =>
           {
               await enterDataMatches.SetFoul(move);
               return Results.Ok();
           })
               .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
            route.MapPost("/set-time-out", async (TeamMovesDto move, [FromServices] IEnterSportMove enterDataMatches) =>
           {
               await enterDataMatches.SetTimeOut(move);
               return Results.Ok();
           })
               .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
            route.MapPost("/set-touch-down", async (TwoPlayerMoveDto move, [FromServices] IEnterSportMove enterDataMatches) =>
            {
                await enterDataMatches.SetTouchdown(move);
                return Results.Ok();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
            route.MapPost("/set-two-players-move", async (TwoPlayerMoveDto move, [FromServices] IEnterSportMove enterDataMatches) =>
            {
                await enterDataMatches.SetTwoPlayersMove(move);
                return Results.Ok();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
             route.MapPost("/set-struggle-fouls",async(TwoPlayerMoveDto move,[FromServices] IEnterSportMove enterDataMatches)=>
            {
                await enterDataMatches.SetStruggleFouls(move);
                return Results.Ok();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
        } 
    }
}