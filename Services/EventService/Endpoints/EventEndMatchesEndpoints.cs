using DB.SportHive.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using  SportHive.Services.Interfaces;

namespace Events.Endpoints
{
    public static class CreateEventService
    { 
        public static void CreateEventsEndpoints(this IEndpointRouteBuilder route)
        {
            route.MapPost("/create-event",async([FromForm] EventDto Event,[FromServices] IEventService eventService)=>
            {
                await eventService.CreateEvent(Event);
            }).DisableAntiforgery() 
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization,Athlete" });
            
            route.MapPost("/inital-create-matches",async([FromBody] Matchs match,[FromServices] ISystemSelectionService matchService)=>
            {
                await matchService.CreateMatchWithSSystem(match);
                return Results.Ok();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
            
            route.MapPost("/add-athletes-em/{IdExtremeMatches}",async(long IdExtremeMatches, [FromBody] List<string> atletes,[FromServices] IEventService eventService)=>
            {
                await eventService.SaveExtreameAtheltes(atletes,IdExtremeMatches);
                return Results.Ok();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
        } 
    }
}