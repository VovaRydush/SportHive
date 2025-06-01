using DB.SportHive.Domain;
using MongoDB.Driver;

namespace SportHive.Services.Interfaces
{
    public interface IMongoDbService
    {
        IMongoDatabase DbContext { get; }
        IMongoCollection<T> GetCollection<T>(string name);
    }

}