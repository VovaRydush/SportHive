namespace SportHive.Services.Interfaces
{
 public interface IJWTService
 { 
    Task<List<string>> GenerateTokens(string Email);
    Task<string> GenereteToken(string Email,string Role,int days);
    Task<bool> CheckRefreshToken(string Email);
 }
}