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

            switch (result.SportType)
            {
                case "Football":
                    result.SportStats = result.SportStats as FootballStats;
                    break;
                case "Struggle":
                    result.SportStats = result.SportStats as StruggleStats;
                    break;
                case "Basketball":
                    result.SportStats = result.SportStats as BasketballStats;
                    break;
                case "Volleyball":
                    result.SportStats = result.SportStats as VolleyballStats;
                    break;
                case "AmericanFootball":
                    result.SportStats = result.SportStats as AmericanFootballStats;
                    break;
                case "Hockey":
                    result.SportStats = result.SportStats as IceHockeyStats;
                    break;
                case "Rugby":
                    result.SportStats = result.SportStats as RugbyStats;
                    break;
                case "Cycling":
                    result.SportStats = result.SportStats as CyclingStats;
                    break;
/*
                case "DistanceRunning":
                    result.SportStats = result.SportStats as Di;
                    break;*/
                case "Rowing":
                    result.SportStats = result.SportStats as RowingStats;
                    break;
                case "Swimming":
                    result.SportStats = result.SportStats as SwimmingStats;
                    break;
                case "WeightliftingMatch":
                    result.SportStats = result.SportStats as WeightliftingStats;
                    break;
                case "Archery":
                    result.SportStats = result.SportStats as ArcheryStats;
                    break;
                case "Box":
                    result.SportStats = result.SportStats as BoxingStats;
                    break;
                case "Checkers":
                    result.SportStats = result.SportStats as CheckersChessStats;
                    break;
                case "Chess":
                    result.SportStats = result.SportStats as CheckersChessStats;
                    break;
                case "CortMatch":
                    result.SportStats = result.SportStats as RacketSportsStats;
                    break;
            };
            
            // Повертаємо JSON без метаданих типу (_t)
            var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore
            });

            return json;
        }
        public async Task<string> GetUserPhoto(string loginUser)
        {
            var path = _appDbContext.UserPhotos.AsNoTracking().Where(x => x.login == loginUser).Select(p => p.ProfilePhoto).FirstOrDefault();
            return await _photoProcessing.GetPhotoBase64Async(path ?? "");
        }
    }
}