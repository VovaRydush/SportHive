using Microsoft.AspNetCore.Mvc;

namespace SportHive.FinalFeatures;

public static class FinalFeatureEndpointExtensions
{
    public static IServiceCollection AddSportHiveStatisticsAndManagement(this IServiceCollection services)
    {
        services.AddScoped<IFinalStatisticsService, FinalStatisticsService>();
        services.AddScoped<IFinalManagementService, FinalManagementService>();
        return services;
    }

    public static IEndpointRouteBuilder MapSportHiveStatisticsAndManagement(this IEndpointRouteBuilder app)
    {
        app.MapGet("/statistics/global", async (
            HttpRequest request,
            IFinalStatisticsService service,
            CancellationToken ct) =>
        {
            var filter = BuildFilter(request);
            return Results.Ok(await service.GetGlobalAsync(filter, ct));
        });

        app.MapGet("/statistics/organization/{login}", async (
            string login,
            HttpRequest request,
            IFinalStatisticsService service,
            CancellationToken ct) =>
        {
            var filter = BuildFilter(request);
            return Results.Ok(await service.GetOrganizationAsync(login, filter, ct));
        });

        app.MapPut("/management/teams/{teamName}", async (
            string teamName,
            TeamUpdateRequest body,
            IFinalManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.UpdateTeamAsync(teamName, body, ct);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        app.MapDelete("/management/teams/{teamName}", async (
            string teamName,
            IFinalManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.DeleteTeamAsync(teamName, ct);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        app.MapPost("/management/teams/{teamName}/athletes", async (
            string teamName,
            TeamAthleteRequest body,
            IFinalManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.AddTeamAthleteAsync(teamName, body, ct);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        app.MapDelete("/management/teams/{teamName}/athletes/{athleteLogin}", async (
            string teamName,
            string athleteLogin,
            IFinalManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.RemoveTeamAthleteAsync(teamName, athleteLogin, ct);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        app.MapPost("/management/organizations/{organizationLogin}/members", async (
            string organizationLogin,
            OrganizationMemberRequest body,
            IFinalManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.AddOrganizationMemberAsync(organizationLogin, body, ct);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        app.MapDelete("/management/organizations/{organizationLogin}/members/{role}/{login}", async (
            string organizationLogin,
            string role,
            string login,
            IFinalManagementService service,
            CancellationToken ct) =>
        {
            var result = await service.RemoveOrganizationMemberAsync(organizationLogin, role, login, ct);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        return app;
    }

    private static StatisticFilter BuildFilter(HttpRequest request)
    {
        return new StatisticFilter
        {
            Season = request.Query["season"].FirstOrDefault(),
            Sport = request.Query["sport"].FirstOrDefault(),
            System = request.Query["system"].FirstOrDefault(),
            Level = request.Query["level"].FirstOrDefault(),
            SortBy = request.Query["sortBy"].FirstOrDefault(),
            Direction = request.Query["direction"].FirstOrDefault()
        };
    }
}
