using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class JWTService : IJWTService
    {
        public Task Generate(string email, string role)
        {
            throw new NotImplementedException();
        }

        public Task UpdatateAccsesToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}