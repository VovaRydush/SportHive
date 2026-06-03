using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;

namespace Events.Endpoints
{
    public static class EventCatalogEndpoints
    {
        public static void EventCatalogEndpoint(this IEndpointRouteBuilder route)
        {
            var catalog = route.MapGroup("/events-catalog");

            catalog.MapGet("", async (
                [FromServices] IEventCatalogService service,
                [FromQuery] string? search,
                [FromQuery] string? typeSport,
                [FromQuery] string? system,
                [FromQuery] string? status,
                [FromQuery] string? login,
                [FromQuery] string? role) =>
            {
                var result = await service.GetCatalogAsync(new EventCatalogQueryDto
                {
                    Search = search,
                    TypeSport = typeSport,
                    System = system,
                    Status = status,
                    Login = login,
                    Role = role
                });

                return Results.Ok(result);
            });

            catalog.MapGet("/{idEvent:long}", async (
                long idEvent,
                [FromServices] IEventCatalogService service,
                [FromQuery] string? login,
                [FromQuery] string? role) =>
            {
                return Results.Ok(await service.GetEventAsync(idEvent, login, role));
            });

            catalog.MapPost("/match-result", async (
                MatchResultSubmitDto dto,
                [FromServices] IEventCatalogService service) =>
            {
                return Results.Ok(await service.SubmitResultAsync(dto));
            });

            catalog.MapPost("/{idEvent:long}/generate-next-round", async (
                long idEvent,
                [FromServices] IEventCatalogService service,
                [FromQuery] string? login,
                [FromQuery] string? role) =>
            {
                return Results.Ok(await service.GenerateNextRoundAsync(idEvent, login, role));
            });

            catalog.MapPost("/{idEvent:long}/rebuild", async (
                long idEvent,
                [FromServices] IEventCatalogService service,
                [FromQuery] string? login,
                [FromQuery] string? role) =>
            {
                return Results.Ok(await service.RebuildEventAsync(idEvent, login, role));
            });
        }
    }
}
