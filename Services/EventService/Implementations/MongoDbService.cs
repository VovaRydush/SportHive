using SportHive.Services.Interfaces;
using MongoDB.Driver;

namespace SportHive.Implementations
{
    public class MongoDbService : IMongoDbService
    {
        private readonly MongoClient _client;
        private readonly IConfiguration _configuration;
        public IMongoDatabase DbContext { get; }

        public MongoDbService(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new MongoClient(_configuration.GetValue<string>("MongoDb:ConnectionString"));
            DbContext = _client.GetDatabase(_configuration.GetValue<string>("MongoDb:Database"));
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return DbContext.GetCollection<T>(name);
        }
    }

}