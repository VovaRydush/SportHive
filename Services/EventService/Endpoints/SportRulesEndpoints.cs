using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;

namespace Events.Endpoints
{
    public static class SportRulesEndpoints
    {
        public static void SportRulesEndpoint(this IEndpointRouteBuilder route)
        {
            var sports = route.MapGroup("/sport-rules");

            sports.MapGet("", ([FromServices] ISportRulesService service) =>
            {
                return Results.Ok(service.GetRules());
            });

            sports.MapGet("/{sport}", (string sport, [FromServices] ISportRulesService service) =>
            {
                return Results.Ok(service.GetRule(sport));
            });

            sports.MapGet("/match/{matchType}/{matchId:long}", async (
                string matchType,
                long matchId,
                [FromServices] ISportRulesService service,
                [FromQuery] string? login,
                [FromQuery] string? role) =>
            {
                return Results.Ok(await service.GetMatchStateAsync(matchType, matchId, login, role));
            });

            sports.MapPost("/match/live-event", async (
                SportLiveEventSubmitDto dto,
                [FromServices] ISportRulesService service) =>
            {
                return Results.Ok(await service.AddLiveEventAsync(dto));
            });

            sports.MapPost("/match/final-result", async (
                SportFinalResultSubmitDto dto,
                [FromServices] ISportRulesService service) =>
            {
                return Results.Ok(await service.SubmitFinalResultAsync(dto));
            });

            sports.MapGet("/validate/{sport}", (
                string sport,
                [FromServices] ISportRulesService service,
                [FromQuery] string score,
                [FromQuery] string first,
                [FromQuery] string second,
                [FromQuery] string? winner) =>
            {
                return Results.Ok(service.ValidateFinalScore(sport, score, first, second, winner));
            });
        }
    }
}
