using SportHive.Services.Interfaces;
using StackExchange.Redis;
namespace SportHive.Implementations
{
    class RedisService : IRedisService
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

        public async Task<string> GetVerifacionCode(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                throw new Exception("key not found or time is out");

            return value!;
        }
        public async Task SetVerifacionCode(string key, string value)
        {
             await _database.StringSetAsync(key, value);
        }
    }
}