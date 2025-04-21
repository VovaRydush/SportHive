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

        public LoginService(AppDbContext context, IJWTService jWTService)
        {
            _jWTService = jWTService;
            _dbcontext = context;
        }
        public async Task<List<string>> Login(UserInfoDto entity)
        {
            var user = await _dbcontext.Users
                                        .AsNoTracking()
                                        .Where(u => u.login == entity.Login)
                                        .Select(u => new { u.HashPassword, u.isEmailConfirmed })
                                        .FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(entity.Password, user.HashPassword))
                throw new Exception("Невірний email або пароль.");

            if (!user.isEmailConfirmed)
                throw new Exception("Підтвердіть email.");
            return await _jWTService.GenerateTokens(entity.Login);
        }

        public async Task LogOut(string login, string refreshToken)
        {
            var user = await _dbcontext.Users
                                    .FirstOrDefaultAsync(u => u.login == login);
            if (refreshToken == user.refreshToken)
            {
                user.refreshToken = null;
                await _dbcontext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Чел ти хто?");
            }
        }


        public Task SetRefreshTokenCookie(HttpContext httpContext, string refreshToken)
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