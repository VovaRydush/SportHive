using SportHive.Services.Interfaces;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

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
            _client = new MongoClient("mongodb://admin:adminpass@127.0.0.1:27017");
            DbContext = _client.GetDatabase("SportHive");
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return DbContext.GetCollection<T>(name);
        }
    }

}