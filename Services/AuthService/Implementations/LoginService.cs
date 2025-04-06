using SportHive.Services.Interfaces;
using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SportHive.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly AppDbContext _dbcontext;
        private readonly IJWTService _jWTService;

        public LoginService(AppDbContext context,IJWTService jWTService)
        {
            _jWTService = jWTService;
            _dbcontext = context;
        }
        public async Task<List<string>> Login(UserInfoDto entity)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == entity.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(entity.Password, user.HashPassword))
                throw new Exception("Невірний email або пароль.");

            if (!user.isEmailConfirmed)
                throw new Exception("Підтвердіть email.");
            return await _jWTService.GenerateTokens(entity.Email);
        }

        public Task LogOut()
        {
            throw new NotImplementedException();
        }


        public  Task SetRefreshTokenCookie(HttpContext httpContext, string refreshToken)
        {
            httpContext.Response.Cookies.Append(
                    "refreshToken",
                    refreshToken,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(14)
                    }
                );
            return Task.CompletedTask;
        }
    }
}