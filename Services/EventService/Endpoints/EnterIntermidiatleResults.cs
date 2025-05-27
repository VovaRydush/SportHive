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
        }
    }
}