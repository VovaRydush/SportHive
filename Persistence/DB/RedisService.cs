using System.Text.Json;
using SportHive.Services.Interfaces;
using StackExchange.Redis;
namespace SportHive.Implementations
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;

        public RedisService(IConnectionMultiplexer database)
        {
            _database = database.GetDatabase();
        }

        public async Task DeleteVerifacionCode(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task<string> GetEntity(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return null;

            return value.ToString()!;
        }

        public async Task<List<string>> GetEntitys(string key)
        {
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
            {
                return null;  
            }

            List<string> result = JsonSerializer.Deserialize<List<string>>(value);
            return result;
        }

        public async Task SetEntity(string key, string value, TimeSpan expiry)
        {
            await _database.StringSetAsync(key, value, expiry);
        }
    }
}