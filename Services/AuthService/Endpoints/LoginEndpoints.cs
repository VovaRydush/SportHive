using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;
using AuthService.Extensions;
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

            route.MapGet("/refresh-token", async (string login, IJWTService jWTService, ILoginService loginService, HttpContext httpContext) =>
            {
                if (await jWTService.CheckRefreshToken(login))
                {
                    List<string> tokens = await jWTService.GenerateTokens(login);
                    await loginService.SetRefreshTokenCookie(httpContext, tokens[1]);
                    return Results.Ok(tokens[0]);
                }
                return Results.Json(new
                {
                    error = "InvalidToken",
                    message = "The access token is expired. Please log in again."
                },statusCode: 401);

            });

            route.MapDelete("/logout", async ([FromBody] string login, ILoginService prorile, HttpContext httpContext) =>
            {
                await prorile.LogOut(login, httpContext.Request.Cookies["refreshToken"]);
                return Results.Ok();
            });

        }
    }
}