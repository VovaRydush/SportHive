using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;

namespace Command.Endpoints
{
    public static class TrainerService
    {
        public static void TrainerEndpoint(this IEndpointRouteBuilder route)
        {
            var trainerRoute = route.MapGroup("/trainer");

            trainerRoute.MapGet("/{login}", async (
                string login,
                [FromServices] ITrainerAthletService trainerService) =>
            {
                return Results.Ok(await trainerService.GetTrainerProfileAsync(login));
            });

            trainerRoute.MapGet("/{login}/teams", async (
                string login,
                [FromServices] ITrainerAthletService trainerService) =>
            {
                return Results.Ok(await trainerService.GetTrainerTeamsAsync(login));
            });
        }
    }
}
