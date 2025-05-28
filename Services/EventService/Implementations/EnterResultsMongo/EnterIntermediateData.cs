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
                    Builders<MatchEvents>.Filter.ElemMatch("athleteSwimming", elemFilter)
                );

                var update = Builders<MatchEvents>.Update
                    .Set("athleteSwimming.$.AthleteName", race.AthleteName)
                    .Set("athleteSwimming.$.DistanceKm", race.DistanceKm)
                    .Set("athleteSwimming.$.FinishTime", race.FinishTime)
                    .Set("athleteSwimming.$.Position", race.Position)
                    .Set("athleteSwimming.$.AvgSpeed", race.AvgSpeed)
                    .Set("athleteSwimming.$.MaxSpeed", race.MaxSpeed)
                    .Set("athleteSwimming.$.DidNotFinish", race.DidNotFinish);

                var result = await _matchEvents.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                {
                    var pushUpdate = Builders<MatchEvents>.Update.Push("athleteSwimming", race);
                    await _matchEvents.UpdateOneAsync(
                        Builders<MatchEvents>.Filter.Eq(x => x.idMatch, race.IdMatch),
                        pushUpdate
                    );
                }
            }
        }

        public async Task SetDistanceRunning(List<DistanceRunning> runnings)
        {
            foreach (var race in runnings)
            {
                var elemFilter = Builders<DistanceRunning>.Filter.Eq(a => a.loginPlayer, race.loginPlayer);

                var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.idMatch, race.idMatch),
                    Builders<MatchEvents>.Filter.ElemMatch("runningAthlets", elemFilter)
                );

                var update = Builders<MatchEvents>.Update
                    .Set("runningAthlets.$.FullNamePlayer", race.FullNamePlayer)
                    .Set("runningAthlets.$.loginPlayer", race.loginPlayer)
                    .Set("runningAthlets.$.timeFinish", race.timeFinish);

                var result = await _matchEvents.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                {
                    var pushUpdate = Builders<MatchEvents>.Update.Push("runningAthlets", race);
                    await _matchEvents.UpdateOneAsync(
                        Builders<MatchEvents>.Filter.Eq(x => x.idMatch, race.idMatch),
                        pushUpdate
                    );
                }
            }
        }

        public Task SetPointsBoxStruggleCort(PointsIntBoxStruggle points)
        {
            throw new NotImplementedException();
        }

        public async Task SetRowingRace(List<RowingRace> races)
        {
            foreach (var race in races)
            {
                var elemFilter = Builders<RowingRace>.Filter.Eq(a => a.AthleteOrTeam, race.AthleteOrTeam);

                var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.idMatch, race.idMatch),
                    Builders<MatchEvents>.Filter.ElemMatch("rowingAtheletes", elemFilter)
                );

                var update = Builders<MatchEvents>.Update
                    .Set("rowingAtheletes.$.DistanceMeters", race.DistanceMeters)
                    .Set("rowingAtheletes.$.ResultTime", race.ResultTime)
                    .Set("rowingAtheletes.$.Place", race.Place)
                    .Set("rowingAtheletes.$.AverageSpeedKmh", race.AverageSpeedKmh)
                    .Set("rowingAtheletes.$.Finished", race.Finished);

                var result = await _matchEvents.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                {
                    var pushUpdate = Builders<MatchEvents>.Update.Push("rowingAtheletes", race);
                    await _matchEvents.UpdateOneAsync(
                        Builders<MatchEvents>.Filter.Eq(x => x.idMatch, race.idMatch),
                        pushUpdate
                    );
                }
            }
        }

        public async Task SetSwimmingResults(List<AthleteSwimming> swimmings)
        {
            foreach (var race in swimmings)
            {
                var elemFilter = Builders<AthleteSwimming>.Filter.Eq(a => a.loginPlayer, race.loginPlayer);

                var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.idMatch, race.idMatch),
                    Builders<MatchEvents>.Filter.ElemMatch("athleteSwimming", elemFilter)
                );

                var update = Builders<MatchEvents>.Update
                    .Set("athleteSwimming.$.time", race.time)
                    .Set("athleteSwimming.$.AvgSpeed", race.AvgSpeed)
                    .Set("athleteSwimming.$.style", race.style);

                var result = await _matchEvents.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                {
                    var pushUpdate = Builders<MatchEvents>.Update.Push("athleteSwimming", race);
                    await _matchEvents.UpdateOneAsync(
                        Builders<MatchEvents>.Filter.Eq(x => x.idMatch, race.idMatch),
                        pushUpdate
                    );
                }
            }
        }

        public async Task SetTeamScore(TeamScoreDto score)
        {
            var filter = Builders<MatchEvents>.Filter.Eq(x => x.idMatch, score.idMatch);
            var update = Builders<MatchEvents>.Update.Set("firstTeamScore", score.Score);
            if(score.Team == 2) update = Builders<MatchEvents>.Update.Set("secondTeamScore", score.Score);
            await _matchEvents.UpdateOneAsync(filter, update);
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

        public Task UpdateBaseballPoint(BaseballPointsDto baseballPoints)
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