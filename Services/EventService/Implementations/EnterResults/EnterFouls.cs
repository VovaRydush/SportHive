using DB.SportHive.Domain;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class EnterFouls : IEnterFouls
    {
        private readonly IMongoDbService _mongoDbService;
        public EnterFouls(IMongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }
        public async Task SetFoul(FoulDto foul)
        {
            // _mongoDbService.dbcontext.GetCollection<>("MatchEvents");
            await Task.CompletedTask;
        }
    }
}