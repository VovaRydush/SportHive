using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class JWTService : IJWTService
    {

        private readonly IConfiguration _config;
        private readonly AppDbContext _db;

        public JWTService(IConfiguration config, AppDbContext db)
        {
            _config = config;
            _db = db;
        }

        public async Task<bool> CheckRefreshToken(string login)
        {
            var refreshToken = await _db.Users.FirstOrDefaultAsync(u => u.login == login);
    
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(refreshToken.refreshToken) as JwtSecurityToken;

                if (jsonToken == null)
                    return true;

                var expirationDate = jsonToken.ValidTo;

                
                return expirationDate < DateTime.UtcNow;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public async Task<List<string>> GenerateTokens(string login)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.login == login);
            var jwtAccsessString = await GenereteToken(login, user.Role, 1);
            var refreshToken = await GenereteToken(login, user.Role, 14);

            user.refreshToken = refreshToken;
            await _db.SaveChangesAsync();

            return new List<string> { jwtAccsessString, refreshToken };
        }

        public async Task<string> GenereteToken(string login, string Role, int days)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credsAccsessToken = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                claims: new[] {
                    new Claim(ClaimTypes.NameIdentifier, login),
                    new Claim(ClaimTypes.Role,Role)
                    },
                expires: DateTime.UtcNow.AddDays(days),
                signingCredentials: credsAccsessToken
            );
            await Task.CompletedTask;
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
