using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class EnterDataMatches : IEnterDataMatches
    {
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        private readonly IMongoCollection<TeamIndivGrid> _playerGrig;
        private readonly DisciplineFactory _disciplineFactory;
        public EnterDataMatches(IMongoDbService mongoDbService, DisciplineFactory disciplineFactory)
        {
            _playerGrig = mongoDbService.GetCollection<TeamIndivGrid>("TeamIndivGrid");
            _disciplineFactory = disciplineFactory;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
        }

        public async Task SaveMatches(TwoPlayerInfo info)
        {
            var sport = _disciplineFactory.CreateIndividual(info);
            _matchEvents.InsertOne(sport);
            await Task.CompletedTask;
        }
        public async Task SaveMatches(TeamInfo info)
        {
            var sport = _disciplineFactory.CreateTeamRecord(info);
            await _matchEvents.InsertOneAsync(sport);
        }
        public async Task SaveMatches(ExtremeIndividualInfo info)
        {
            var sport = _disciplineFactory.CreateExteme(info);
            _matchEvents.InsertOne(sport);
            await Task.CompletedTask;
        }
        public async Task SaveMatches(QualificationGrid grid)
        {
            await _playerGrig.InsertOneAsync(grid);
        }
    }
}