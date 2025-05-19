using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class EnterDataMatches : IEnterDataMatches
    {
        private readonly  IMongoCollection<MatchEvents> _matchEvents;
        private readonly DisciplineFactory _disciplineFactory;
        public EnterDataMatches(IMongoDbService mongoDbService,DisciplineFactory disciplineFactory)
        {
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
            _matchEvents.InsertOne(sport);
            await Task.CompletedTask;
        }
        public async Task SaveMatches(ExtremeIndividualInfo info)
        {
            var sport = _disciplineFactory.CreateExteme(info);
            _matchEvents.InsertOne(sport);
            await Task.CompletedTask;
        }

        public async Task SetFoul(FoulDto foul)
        {
            //_matchEvents.InsertMany();
            await Task.CompletedTask;
        }
    }
}