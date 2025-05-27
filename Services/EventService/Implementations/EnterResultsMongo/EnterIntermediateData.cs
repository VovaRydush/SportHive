using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class EnterIntermediateData : IEnterIntermediateData
    {
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        public EnterIntermediateData(IMongoDbService mongoDbService)
        {
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
        }
        public async Task SetCyclingRace(List<CyclingRace> races)
        {
            foreach (var race in races)
            {
                var elemFilter = Builders<CyclingRace>.Filter.Eq(a => a.loginPlayer, race.loginPlayer);

                var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.idMatch, race.IdMatch),
                    Builders<MatchEvents>.Filter.ElemMatch("atheltesMoves", elemFilter)
                );

                var update = Builders<MatchEvents>.Update
                    .Set("atheltesMoves.$.AthleteName", race.AthleteName)
                    .Set("atheltesMoves.$.DistanceKm", race.DistanceKm)
                    .Set("atheltesMoves.$.FinishTime", race.FinishTime)
                    .Set("atheltesMoves.$.Position", race.Position)
                    .Set("atheltesMoves.$.AvgSpeed", race.AvgSpeed)
                    .Set("atheltesMoves.$.MaxSpeed", race.MaxSpeed)
                    .Set("atheltesMoves.$.DidNotFinish", race.DidNotFinish);

                var result = await _matchEvents.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                {
                    var pushUpdate = Builders<MatchEvents>.Update.Push("atheltesMoves", race);
                    await _matchEvents.UpdateOneAsync(
                        Builders<MatchEvents>.Filter.Eq(x => x.idMatch, race.IdMatch),
                        pushUpdate
                    );
                }
            }

        }

        public Task SetDistanceRunning(List<DistanceRunning> runnings)
        {
            throw new NotImplementedException();
        }

        public Task SetPointsBoxStruggleCort(PointsIntBoxStruggle points)
        {
            throw new NotImplementedException();
        }

        public Task SetRowingRace(List<RowingRace> races)
        {
            throw new NotImplementedException();
        }

        public Task SetSwimmingResults(List<AthleteSwimming> swimmings)
        {
            throw new NotImplementedException();
        }

        public Task SetTeamScore(TeamScoreDto score) // тут і подумати над баскетболом
        {
            throw new NotImplementedException();
        }

        public Task SetWeightliftingResults(Weightlifting result)
        {
            throw new NotImplementedException();
        }

        public Task SetWinnerBoxStruggle(BoxWinnerDto winner)
        {
            throw new NotImplementedException();
        }

        public Task SetWinnerChessCheckers(ChessWinnerDto chessWinner)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBaseballMatch(BaseballEvent move)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBaseballPoint()
        {
            throw new NotImplementedException();
        }

        public Task UpdateCheckersMove(CheckersNotationDto notationDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateChessMove(ChessNotationDto notationDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCortSportMatch()
        {
            throw new NotImplementedException();
        }
    }
}