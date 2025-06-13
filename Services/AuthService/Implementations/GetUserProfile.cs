using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class GetUserProfile : IGetUserProfile
    {
        private readonly IPhotoProcessing _photoProcessing;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly AppDbContext _appDbContext;
        public GetUserProfile(IPhotoProcessing photoProcessing, IMongoDbService mongoDbService, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
            _photoProcessing = photoProcessing;
        }
        public async Task<string> GetAllInfoUser(string loginUser)
        {
            var filter = Builders<AthleteProfile>.Filter.Eq(x => x.login, loginUser);
            var result = await _playerProfile.Find(filter).FirstOrDefaultAsync();

            if (result == null)
                return null;

            // Явно кастимо SportStats до конкретного типу, якщо потрібно
            result.SportStats = result.SportStats as FootballStats;

            // Повертаємо JSON без метаданих типу (_t)
            var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore
            });

            return json;
        }


        public Task<List<string>> GetLastMatches(string loginUser)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetUserPhoto(string loginUser)
        {
            var path = _appDbContext.UserPhotos.AsNoTracking().Where(x => x.login == loginUser).Select(p => p.ProfilePhoto).FirstOrDefault();
            return await _photoProcessing.GetPhotoBase64Async(path ?? "");
        }
    }
}