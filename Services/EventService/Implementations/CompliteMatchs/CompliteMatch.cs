using System.Data.Common;
using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class CompliteMatch : ICompliteMatch
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        private readonly IEventService _eventService;
        public CompliteMatch(IMongoDbService mongoDbService, AppDbContext appDbContext, IEventService eventService)
        {
            _eventService = eventService;
            _appDbContext = appDbContext;
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
        }
        public async Task CompliteArchery(CompliteArcheryDto data)
        {
            await CompliteExtremeMatch(_appDbContext,data.MatchInfo);
            if (data.MatchInfo.location != null) _eventService.SaveLocation(_appDbContext, new LocationDto
            {
                address = data.MatchInfo.location.LocationName,
                lat = data.MatchInfo.location.Latitude,
                lng = data.MatchInfo.location.Longitude,
            });

            var filter = Builders<MatchEvents>.Filter.Eq(x => x.idMatch, data.MatchInfo.idMatch);
            var update = Builders<MatchEvents>.Update
                    .Set("BowType", data.BowType)
                    .Set("Distance", data.Distance)
                    .Set("CompetitionType", data.CompetitionType);

            await _matchEvents.UpdateOneAsync(filter, update);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task CompliteChess(CompliteChessDto data)
        {
            await CompliteIndividualMatch(_appDbContext,data.MatchInfo);
            if (data.MatchInfo.location != null) _eventService.SaveLocation(_appDbContext, new LocationDto
            {
                address = data.MatchInfo.location.LocationName,
                lat = data.MatchInfo.location.Latitude,
                lng = data.MatchInfo.location.Longitude,
            });
            var filter = Builders<MatchEvents>.Filter.Eq(x => x.idMatch, data.MatchInfo.idMatch);
            var update = Builders<MatchEvents>.Update
                    .Set("TimeControl", data.TimeControl);

            await _matchEvents.UpdateOneAsync(filter, update);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task CompliteCycling(CompliteCyclingDto data)
        {
            await CompliteExtremeMatch(_appDbContext,data.MatchInfo);
            if (data.MatchInfo.location != null) _eventService.SaveLocation(_appDbContext, new LocationDto
            {
                address = data.MatchInfo.location.LocationName,
                lat = data.MatchInfo.location.Latitude,
                lng = data.MatchInfo.location.Longitude,
            });
            var filter = Builders<MatchEvents>.Filter.Eq(x => x.idMatch, data.MatchInfo.idMatch);
            var update = Builders<MatchEvents>.Update
                    .Set("RaceType", data.RaceType);

            await _matchEvents.UpdateOneAsync(filter, update);
        }
        public async Task CompliteRowing(CompliteRowingDto data)
        {
            await CompliteExtremeMatch(_appDbContext,data.MatchInfo);
            if (data.MatchInfo.location != null) _eventService.SaveLocation(_appDbContext, new LocationDto
            {
                address = data.MatchInfo.location.LocationName,
                lat = data.MatchInfo.location.Latitude,
                lng = data.MatchInfo.location.Longitude,
            });
            var filter = Builders<MatchEvents>.Filter.Eq(x => x.idMatch, data.MatchInfo.idMatch);
            var update = Builders<MatchEvents>.Update
                    .Set("BoatType", data.BoatType);

            await _matchEvents.UpdateOneAsync(filter, update);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task CompliteWeightlifting(List<CompliteWeightliftingDto> data)
        {
            await CompliteExtremeMatch(_appDbContext, data[0].MatchInfo);
            if (data[0].MatchInfo.location != null) _eventService.SaveLocation(_appDbContext, new LocationDto
            {
                address = data[0].MatchInfo.location.LocationName,
                lat = data[0].MatchInfo.location.Latitude,
                lng = data[0].MatchInfo.location.Longitude,
            });
            foreach (var data1 in data)
            {
                var filter = Builders<MatchEvents>.Filter.And(
                    Builders<MatchEvents>.Filter.Eq(x => x.idMatch, data1.MatchInfo.idMatch),
                    Builders<MatchEvents>.Filter.ElemMatch("liftingsAthlete",
                        Builders<Weightlifting>.Filter.And(
                            Builders<Weightlifting>.Filter.Eq(x => x.FullNamePlayer, data1.FullNamePlayer),
                            Builders<Weightlifting>.Filter.Eq(x => x.desipline, data1.desipline),
                            Builders<Weightlifting>.Filter.Eq(x => x.WeightCategory, data1.WeightCategory)
                        )
                    )
                );
                var update = Builders<MatchEvents>.Update.Combine(
                    Builders<MatchEvents>.Update.Set("liftingsAthlete.$.Weight", data1.Weight),
                    Builders<MatchEvents>.Update.Set("liftingsAthlete.$.WeightCategory", data1.WeightCategory),
                    Builders<MatchEvents>.Update.Set("liftingsAthlete.$.desipline", data1.desipline)
                );

                await _matchEvents.UpdateOneAsync(filter, update);
            }
            await _appDbContext.SaveChangesAsync();
        }

        public async Task CompliteExtremeMatch(AppDbContext appDbContext, CompliteMatchInfo data)
        {
            await appDbContext.ExtremeMatches
           .Where(m => m.IdExtremeMatches == data.idMatch)
           .ExecuteUpdateAsync(s =>
               s.SetProperty(m => m.AddInformation, m => data.AddInformation)
               .SetProperty(m => m.DataMatch, data.dateStart)
               .SetProperty(m => m.TimeMatch, data.timeStart)
               .SetProperty(m => m.LocationName, data.location.LocationName));
            await _appDbContext.SaveChangesAsync();
        }

        public async Task CompliteIndividualMatch(AppDbContext appDbContext, CompliteMatchInfo data)
        {
            await appDbContext.IndividualMatches
                .Where(m => m.IdIndividualMatch == data.idMatch)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(m => m.AddInformation, m => data.AddInformation)
                        .SetProperty(m => m.DataMatch, data.dateStart)
                        .SetProperty(m => m.TimeMatch, data.timeStart)
                        .SetProperty(m => m.LocationName, m => data.location != null ? data.location.LocationName : m.LocationName)
                                   );
            await _appDbContext.SaveChangesAsync();
        }

        public async Task CompliteTeamMatch(AppDbContext appDbContext, CompliteMatchInfo data)
        {
            await appDbContext.TeamMatches
            .Where(m => m.IdTeamMatch == data.idMatch)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(m => m.AddInformation, m => data.AddInformation)
                .SetProperty(m => m.DataMatch, data.dateStart)
                .SetProperty(m => m.TimeMatch, data.timeStart)
                .SetProperty(m => m.LocationName, data.location.LocationName));
            await _appDbContext.SaveChangesAsync();
        }
    }
}