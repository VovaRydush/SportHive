using DB.SportHive.Persistence;
using SportHive.Implementations.Background;

namespace Events.Endpoints
{
    public static class MatchStatusMaintenanceEndpoints
    {
        public static void MatchStatusMaintenanceEndpoint(this IEndpointRouteBuilder route)
        {
            route.MapPost("/maintenance/matches/normalize-statuses", async (AppDbContext db) =>
            {
                var changed = await MatchStatusNormalizer.NormalizeAsync(db);
                return Results.Ok(new { changed });
            });
        }
    }
}
