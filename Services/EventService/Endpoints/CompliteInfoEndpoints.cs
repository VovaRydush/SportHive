using DB.SportHive.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;

namespace Events.Endpoints
{
    public static class CompliteInfoEndpoints
    {
        public static void CompliteInfoEndpoint(this IEndpointRouteBuilder route)
        {
            route.MapPatch("/complite-chess-info", async ([FromBody] CompliteChessDto data, [FromServices] ICompliteMatch compliteMatch) =>
            {
                await compliteMatch.CompliteChess(data);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/complite-archery-info", async ([FromBody] CompliteArcheryDto data, [FromServices] ICompliteMatch compliteMatch) =>
            {
                await compliteMatch.CompliteArchery(data);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/complite-cycling-info", async ([FromBody] CompliteCyclingDto data, [FromServices] ICompliteMatch compliteMatch) =>
            {
                await compliteMatch.CompliteCycling(data);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/complite-rowing-info", async ([FromBody] CompliteRowingDto data, [FromServices] ICompliteMatch compliteMatch) =>
            {
                await compliteMatch.CompliteRowing(data);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });

            route.MapPatch("/complite-weightlifting-info", async ([FromBody] List<CompliteWeightliftingDto> data, [FromServices] ICompliteMatch compliteMatch) =>
            {
                await compliteMatch.CompliteWeightlifting(data);
            }).DisableAntiforgery()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
        }
    }
}
