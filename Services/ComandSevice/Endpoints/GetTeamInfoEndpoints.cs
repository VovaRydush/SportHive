using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Command.Endpoints
{
    public static class TeamInfoService
    {
        public static void GetInfoTeam(this IEndpointRouteBuilder route)
        {
            route.MapGet("/athelets-team", async (string team, [FromServices] IGetInfoTeam teamService) =>
            {
                return Results.Ok(await teamService.GetAthetesAsync(team));
            });
            route.MapGet("/photo/{filePath}", async (string filePath, IPhotoProcessing photoProcessing) =>
            {
                var (fileContent, mimeType) = await photoProcessing.GetPhotoAsync(filePath);
                return Results.File(fileContent, mimeType);
            });
            route.MapGet("/team/{NameTeam}",async(string NameTeam,[FromServices] IGetInfoTeam teamService)=>
            {
                return Results.Ok(await teamService.GetTeamInfo(NameTeam));
            });
        }
    }
}