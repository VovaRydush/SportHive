using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using DB.SportHive.Persistence;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class GetMatchesPlayer : IGetMatchesPlayer
    {
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        private readonly AppDbContext _appDbContext;
        public readonly IPhotoProcessing _photoProcessing;
        public GetMatchesPlayer(IMongoDbService mongoDbService, AppDbContext appDbContext,IPhotoProcessing photoProcessing)
        {
            _photoProcessing = photoProcessing;
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("AthleteProfile");
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _appDbContext = appDbContext;
        }
        public Task<List<string>> GetExetrmeMatch(string loginUser)
        {
            throw new NotImplementedException();
        }

        public Task<TeamIndivMatch> GetIndividualMatches(string loginUser)
        {
            throw new NotImplementedException();
        }

        public Task<TeamIndivMatch> GetTeamMatches(string loginUser)
        {
            throw new NotImplementedException();
        }
    }
}