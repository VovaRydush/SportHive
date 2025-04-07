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
            route.MapPost("/login", async (HttpContext httpContext, [FromBody] UserInfoDto user, ILoginService loginService) =>
            {
                List<string> tokens = await loginService.Login(user);
                await loginService.SetRefreshTokenCookie(httpContext, tokens[1]);
                return Results.Ok(tokens[0]);
            });

            route.MapGet("/refresh-token", async (string Email, IJWTService jWTService, ILoginService loginService, HttpContext httpContext) =>
            {
                if (await jWTService.CheckRefreshToken(Email))
                {
                    List<string> tokens = await jWTService.GenerateTokens(Email);
                    await loginService.SetRefreshTokenCookie(httpContext, tokens[0]);
                    return Results.Ok(tokens[1]);
                }
                return Results.Json(new
                {
                    error = "InvalidToken",
                    message = "The access token is expired. Please log in again."
                },statusCode: 401);

            });

            route.MapDelete("/logout", async ([FromBody] string Email, ILoginService prorile, HttpContext httpContext) =>
            {
                await prorile.LogOut(Email, httpContext.Request.Cookies["refreshToken"]);
                return Results.Ok();
            });

        }
    }
}