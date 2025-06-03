using DB.SportHive.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;

namespace Events.Endpoints
{
    public static class UsersProfileEndpoints
    {
        public static void UsersProfileEndpoint(this IEndpointRouteBuilder route)
        {
            route.MapPatch("/change-position", async ([FromBody] AthleteValuesDto newPosition, [FromServices] INuclearRap athleteProfile) =>
            {
                await athleteProfile.ChangePosition(newPosition);
                return Results.Ok();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
            route.MapPatch("/set-weight-category", async ([FromBody] AthleteValuesDto weigntCategory, [FromServices] INuclearRap athleteProfile) =>
            {
                await athleteProfile.SetWeightCategory(weigntCategory);
                return Results.Ok();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Trainer,Organization" });
        }
    }
}