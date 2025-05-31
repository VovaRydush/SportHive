using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;
using SportHive.Implementations;
using DB.SportHive.Persistence;
using DB.SportHive.Domain;
using System.Threading.Tasks;

namespace AuthService.Test.TestInterface
{
    public class JWTServiceTests
    {
        private JWTService CreateService(out AppDbContext context)
        {
            // Setup in-memory DB
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new AppDbContext(options);

            // Setup config
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Jwt:Key", "1234567890123456"}, // Мінімум 16 символів
                    {"Jwt:Issuer", "TestIssuer"}
                })
                .Build();

            return new JWTService(config, context);
        }

        [Fact]
        public void GenerateToken_ReturnsValidJwtToken()
        {
            var service = CreateService(out _);

            string token = service.GenereteToken("testuser", "Admin", 30);
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public async Task CheckRefreshToken_ValidToken_ReturnsTrue()
        {
            var service = CreateService(out var context);

            var token = service.GenereteToken("admin", "User", 30);

            var user = new User
            {
                login = "admin",
                HashPassword = "hashedpassword",
                refreshToken = token
            };

            context.Users.Add(user);
            context.SaveChanges();

            bool result = await service.CheckRefreshToken(token);
            Assert.True(result);
        }

        [Fact]
        public async Task CheckRefreshToken_InvalidToken_ReturnsFalse()
        {
            var service = CreateService(out var context);

            var user = new User
            {
                login = "admin",
                HashPassword = "hashedpassword",
                refreshToken = "valid.token.but.not.matching"
            };

            context.Users.Add(user);
            context.SaveChanges();

            bool result = await service.CheckRefreshToken("invalid.token");
            Assert.False(result);
        }
    }
}
