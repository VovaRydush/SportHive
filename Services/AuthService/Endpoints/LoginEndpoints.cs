using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;
using SportHive.Extensions;
using SportHive.Services.Interfaces;

namespace AuthService.Endpoints
{
    public static class LoginService
    {
        public static void UserLoginEndpoint(this IEndpointRouteBuilder route)
        {
            route.MapGet("/login", async (HttpContext httpContext, [FromBody] UserInfoDto user, ILoginService loginService) =>
            {
                List<string> tokens = await loginService.Login(user);
                await loginService.SetRefreshTokenCookie(httpContext,tokens[0]);
                return Results.Ok(tokens[1]);
            });
        }
    }
}