using DB.SportHive.Domain;
using MongoDB.Driver;

namespace SportHive.Services.Interfaces
{
    public interface IMongoDbService
    {
        IMongoDatabase dbcontext {get;set;}
    }
}