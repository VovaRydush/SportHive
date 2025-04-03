namespace SportHive.Services.Interfaces
{
 public interface IRedisService
 { 
     Task SetVerifacionCode(string key, string value);
     Task<string> GetVerifacionCode(string key);
     Task DeleteVerifacionCode(string key);
 }
}