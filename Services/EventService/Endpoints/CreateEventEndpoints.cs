using DB.SportHive.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportHive.Implementations;
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
            
            route.MapPost("/create-individual-match",async([FromBody] IndividualMatchDto match,[FromServices] IEventService eventService)=>
            {
                await eventService.AddIndividualMathDto(match);
                return Results.Ok();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });;
        } 
    }
}