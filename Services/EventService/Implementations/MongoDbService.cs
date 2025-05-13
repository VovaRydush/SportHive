using SportHive.Services.Interfaces;
using MongoDB.Driver;

namespace SportHive.Implementations
{
    public class MongoDbService : IMongoDbService
    {
        private readonly MongoClient client;
        public IMongoDatabase dbcontext;
        private readonly IConfiguration _configuration;

        IMongoDatabase IMongoDbService.dbcontext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MongoDbService(IConfiguration configuration)
        {
            _configuration = configuration;
            client = new MongoClient(_configuration.GetValue<string>("MongoDb:ConnectionString"));
            dbcontext = client.GetDatabase(_configuration.GetValue<string>("MongoDb:Database"));
        }
        
    }
}