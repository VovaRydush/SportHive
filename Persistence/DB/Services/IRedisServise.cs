namespace SportHive.Services.Interfaces
{
 public interface IRedisService
 { 
     Task SetEntity(string key, string value,TimeSpan expiry);
     Task<string> GetEntity(string key);
     Task<List<string>> GetEntitys(string key);
     Task DeleteVerifacionCode(string key);
 }
}