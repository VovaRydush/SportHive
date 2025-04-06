
using DB.SportHive.Domain;
using Microsoft.AspNetCore.Mvc;
using SportHive.Services.Interfaces;

namespace AuthService.Endpoints
{
    public static class UserManipuleteEndpoints
    {
        public static void UserManipuleteEndpoint(this IEndpointRouteBuilder route)
        {
            route.MapPost("/send-recovery-email",async([FromBody] string Email,IProfileManipulete profile)=>{
                await profile.SendVereficationCode(Email);
            });
            route.MapPost("/change-password",async([FromBody] UserInfoDto user,IProfileManipulete profile)=>{
               // додати перевіку чи правельний пароль і тд(якщо правельно видалити з кешу)
                await profile.PasswordRecovery(user.Email,user.Password);
            });
        }
    }
}