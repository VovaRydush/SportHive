using SportHive.Services.Interfaces;
using MongoDB.Driver;

namespace SportHive.Implementations
{
    public class MongoDbService : IMongoDbService
    {
        private readonly MongoClient client;
        private readonly IMongoDatabase _dbcontext;
        private readonly IConfiguration _configuration;
        public MongoDbService(IConfiguration configuration)
        {
            _configuration = configuration;
            client = new MongoClient(_configuration.GetValue<string>("MongoDb:ConnectionString"));
            _dbcontext = client.GetDatabase(_configuration.GetValue<string>("MongoDb:Database"));
        }
        
    }
}