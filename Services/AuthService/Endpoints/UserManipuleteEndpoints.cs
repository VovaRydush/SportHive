
using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;

namespace AuthService.Endpoints
{
    public static class UserManipuleteEndpoints
    {
        public static void UserManipuleteEndpoint(this IEndpointRouteBuilder route)
        {
            route.MapPost("/send-recovery-email", async ([FromBody] string Email, IProfileManipulete profile) =>
            {
                await profile.SendVereficationCode(Email);
            });
            
            route.MapGet("/check-code-recavery", async ([FromBody] UserVerificationDto user, [FromServices] IUserRegistration _checkemail) =>
            {
                await _checkemail.VeryfyEmail(user);
                return Results.Ok();
            });

            route.MapPost("/change-password", async ([FromBody] UserInfoDto user, IProfileManipulete profile) =>
            {
                await profile.PasswordRecovery(user.Email, user.Password);
            });
        }
    }
}