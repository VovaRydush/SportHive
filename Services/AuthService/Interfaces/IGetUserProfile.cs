using DB.SportHive.Domain;
using StackExchange.Redis;

namespace SportHive.Services.Interfaces
{
    public interface IGetUserProfile
    {
        Task<string> GetUserPhoto(string loginUser);
        Task<string> GetAllInfoUser(string loginUser);
        Task<List<string>> GetLastMatches(string loginUser);
    }
}