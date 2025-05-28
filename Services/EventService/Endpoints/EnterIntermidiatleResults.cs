using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using  SportHive.Services.Interfaces;

namespace Events.Endpoints
{
    public static class EnterIntermidiatleResults
    {
        public static void EnterIntermidiatleResult(this IEndpointRouteBuilder route)
        {
            route.MapPatch("/set-cycling-race", async ([FromBody] List<CyclingRace> races, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.SetCyclingRace(races);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/set-swimming-results", async ([FromBody] List<AthleteSwimming> races, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.SetSwimmingResults(races);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/set-rowing-race", async ([FromBody] List<RowingRace> races, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.SetRowingRace(races);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/set-distance-running", async ([FromBody] List<DistanceRunning> races, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.SetDistanceRunning(races);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
            
             route.MapPatch("/SetTeamScore", async ([FromBody] TeamScoreDto scoreDto, [FromServices] IEnterIntermediateData enterData) =>
            {
                await enterData.SetTeamScore(scoreDto);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
        }
    }
}