using SportHive.Services.Interfaces;

namespace Events.Endpoints
{
    public static class MatchCompletionEndpoints
    {
        public static void MatchCompletionEndpoint(this IEndpointRouteBuilder route)
        {
            route.MapPost("/matches/complete", async (
                CompleteMatchRequest request,
                IMatchCompletionService completionService) =>
            {
                var result = await completionService.CompleteMatchAsync(request);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            });

            route.MapPost("/matches/normalize-completion", async (IMatchCompletionService completionService) =>
            {
                var changed = await completionService.NormalizeAllAsync();
                return Results.Ok(new { changed });
            });
        }
    }
}
