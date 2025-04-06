using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;
using SportHive.Extensions;
using SportHive.Services.Interfaces;

namespace AuthService.Endpoints
{
    public static class AuthService
    {
        public static void UserRegisterEndpoint(this IEndpointRouteBuilder route)
        {
           
            route.MapPost("/registr", async (UserInfoDto user, IUserRegistration userService) =>
            {
                await userService.Registration(user);
                return Results.Ok("User registered successfully!");
            });
            
            route.MapPost("/complite-profile", async([FromForm] RoleInfoDto user,IUserRegistration userService)=>{
                await userService.ComplitePrifile(user);
                return Results.Ok();
            }).AllowAnonymous().DisableAntiforgery(); ;

            route.MapPost("/complite-profile-organization", async([FromForm] OrganizationInfoDto user,IUserRegistration userService)=>{
                await userService.ComplitePrifileOrganization(user);
                return Results.Ok();
            }).AllowAnonymous().DisableAntiforgery(); ;

            route.MapGet("/verify", async ([FromBody] UserVerificationDto info, IUserRegistration userService) =>
            {
                await userService.VeryfyEmail(info);
                return Results.Ok();
            });
        }
    }
}