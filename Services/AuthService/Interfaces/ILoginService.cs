using DB.SportHive.Domain;
namespace SportHive.Services.Interfaces
{
 public interface ILoginService
 { 
    Task<List<string>> Login(UserInfoDto entity);
    Task SetRefreshTokenCookie(HttpContext httpContext, string refreshToken);
    Task LogOut();
 }
}