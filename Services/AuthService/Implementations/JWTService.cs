using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        public async Task<List<string>> GenerateTokens(string Email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == Email);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credsAccsessToken = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtAccsessToken = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                claims: new[] { 
                    new Claim(ClaimTypes.NameIdentifier, Email),
                    new Claim(ClaimTypes.Role,user.Role) 
                    },
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credsAccsessToken
            );
            var jwtAccsessString = new JwtSecurityTokenHandler().WriteToken(jwtAccsessToken);
            
            var credsRefreshToken = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var JwtRefreshToken = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                claims: new[] { 
                    new Claim(ClaimTypes.NameIdentifier, Email),
                    new Claim(ClaimTypes.Role,user.Role) 
                    },
                expires: DateTime.UtcNow.AddDays(14),
                signingCredentials: credsRefreshToken
            );
            var refreshToken = new JwtSecurityTokenHandler().WriteToken(JwtRefreshToken);
            
            user.refreshToken = refreshToken;
            await _db.SaveChangesAsync();

            return new List<string> { jwtAccsessString, refreshToken };
        }

        public Task UpdatateAccsesToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
