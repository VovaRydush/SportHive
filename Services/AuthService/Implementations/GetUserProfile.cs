using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
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
        public async Task<AthleteProfile> GetAllInfoUser(string loginUser)
        {
            var filter = Builders<AthleteProfile>.Filter.Eq(x => x.login, loginUser);
            return await _playerProfile.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<string> GetUserPhoto(string loginUser)
        {
            var path = _appDbContext.UserPhotos.AsNoTracking().Where(x => x.login == loginUser).Select(p=>p.ProfilePhoto).FirstOrDefault();
            return await _photoProcessing.GetPhotoBase64Async(path ?? "");
        }
    }
}