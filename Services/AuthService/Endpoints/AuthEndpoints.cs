using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;

namespace AuthService.Endpoints
{
    public static class AuthService
    {
        public static void UserRegisterEndpoint(this IEndpointRouteBuilder route)
        {
           
            route.MapPost("/registr", async (UserInfoDto user, IUserRegistration userService) =>
            {
                await userService.Registration(user.Email, user.Password);
                return Results.Ok("User registered successfully!");
            });

            route.MapGet("/verify", async ([FromBody] UserVerificationDto info, IUserRegistration userService) =>
            {
                await userService.VeryfyEmail(info);
                return Results.Ok();
            });
        }
    }
}