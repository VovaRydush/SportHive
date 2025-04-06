namespace SportHive.Services.Interfaces
{
 public interface IJWTService
 { 
    Task<List<string>> GenerateTokens(string userId);
    Task UpdatateAccsesToken(string refreshToken);
 }
}