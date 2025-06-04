using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class NuclearRap : INuclearRap
    {
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public NuclearRap(IMongoDbService mongoDbService)
        {
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
        }
        public async Task ChangePosition(AthleteValuesDto position)
        {
            var filter = Builders<AthleteProfile>.Filter.And(
                  Builders<AthleteProfile>.Filter.Eq(x => x.login, position.loginPlayer)
              );
            var update = Builders<AthleteProfile>.Update.Combine(
                Builders<AthleteProfile>.Update.Set("Position", position.value)
            );
            await _playerProfile.UpdateOneAsync(filter, update);
        }

        public async Task<long> CountWinIndividual(string loginPlayer)
        {
            var filter = Builders<MatchEvents>.Filter.Eq(x => x.NameWinner, loginPlayer);
            return await _matchEvents.CountDocumentsAsync(filter); 
        }
        public async Task<long> CountMatchs(string loginPlayer)
        {
            var filter = Builders<MatchEvents>.Filter.AnyEq(x => x.composition, loginPlayer);
            return await _matchEvents.CountDocumentsAsync(filter);
        }
        public async Task SetWeightCategory(AthleteValuesDto newWeight)
        {
            var update = Builders<AthleteProfile>.Update
                .Set("SportStats.WeightCategory", newWeight.value)
                .Set(x => x.dateLastUpdate, DateTime.UtcNow);

            await _playerProfile.UpdateOneAsync(
                filter: Builders<AthleteProfile>.Filter.Eq(x => x.login, newWeight.loginPlayer),
                update: update
            );
        }

        public async Task<long> CountLossIndividual(string loginPlayer)
        {
            var filter = Builders<MatchEvents>.Filter.Eq(x => x.NameLosser, loginPlayer);
            return await _matchEvents.CountDocumentsAsync(filter);
        }

        public async Task<long> CountDrawIndividual(string loginPlayer)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Ne(x => x.Draws, null),
                    Builders<MatchEvents>.Filter.AnyEq(x => x.composition, loginPlayer)
                );
            return await _matchEvents.CountDocumentsAsync(filter);
        }

        public async Task<long> CountWinTeam(AthletesTeamDto athlet)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.NameWinner, athlet.NameTeam),
                    Builders<MatchEvents>.Filter.AnyEq(x => x.composition, athlet.loginPlayer)
                );
            return await _matchEvents.CountDocumentsAsync(filter);

        }

        public async Task<long> CountLossTeam(AthletesTeamDto athlet)
        {
            var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.NameLosser, athlet.NameTeam),
                    Builders<MatchEvents>.Filter.AnyEq(x => x.composition, athlet.loginPlayer)
                );
            return await _matchEvents.CountDocumentsAsync(filter);
        }
    }
}