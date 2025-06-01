using DB.SportHive.Domain;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class CompliteUserProfile : ICompliteUserProfile
    {
        private readonly UserProfileFactory _userProfileFactory;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        public CompliteUserProfile(IMongoDbService mongoDbService,UserProfileFactory userProfileFactory)
        {
             _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
            _userProfileFactory = userProfileFactory;
        }
        public async Task CreateProfileInMongoAsync(string fullName, string Login, string sportType)
        {
            var profile = _userProfileFactory.CreateUserProfile(sportType);
            await _playerProfile.InsertOneAsync(new AthleteProfile(fullName, Login, sportType)
            {
                SportStats = profile 
            });
        }
    }
}