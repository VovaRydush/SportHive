using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;

namespace AuthService.Endpoints
{
    public static class UserProfileEndpoints
    {
        public static void UserProfileEndpoint(this IEndpointRouteBuilder route)
        {
            route.MapGet("/get-user-photo/{login}", async (string login, [FromServices] IGetUserProfile _userProfile) =>
            {
                return Results.Ok(await _userProfile.GetUserPhoto(login));
            });
            route.MapGet("/get-statistic-info/{login}", async (string login, [FromServices] IGetUserProfile _userProfile) =>
            {
                return Results.Ok(await _userProfile.GetAllInfoUser(login));
            });
            route.MapGet("/get-search-athlete/{FullName}", async ([FromQuery] string FullName, [FromServices] IAthleteService _userProfile) =>
            {
                Console.WriteLine("NNENNENENNENE");
                return Results.Ok(await _userProfile.SearchAthletesAsync(FullName));
            });
        }
    }
}