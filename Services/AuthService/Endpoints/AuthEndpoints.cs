using DB.SportHive.Domain;
using SportHive.Services.Interfaces;

namespace AuthService.Endpoints
{
    public static class AuthService
    {
        public static void UserRegisterEndpoint(this IEndpointRouteBuilder route)
        {
           
            route.MapPost("/registr", async (UserInfoDto user, IUserService userService) =>
            {
                await userService.Registration(user.Email, user.Password);
                return Results.Ok("User registered successfully!");
            });

            route.MapGet("/users", async (IUserService userService) =>
            {
                List<User> users = await userService.GetAllUsers();
                return Results.Ok(users);
            });

            route.MapGet("/verify", async (string token, IUserService userService) =>
            {
                await userService.VeryfyEmail(token);
                return Results.Ok("Верефікація пройшла успішно!");
            });
        }
    }
}