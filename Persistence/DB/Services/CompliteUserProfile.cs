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
        public async Task CreateProfileInMongoAsync(string fullName, string Login, string sportType,DateTime dataBirhstay)
        {
            var profile = _userProfileFactory.CreateUserProfile(sportType);
            _playerProfile.InsertOne(new AthleteProfile(fullName, Login, sportType,dataBirhstay)
            {
                SportStats = profile
            });
            await Task.CompletedTask;
        }
    }
}