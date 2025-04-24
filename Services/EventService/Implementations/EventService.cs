using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Exceptions;

//using MongoDB.Driver.Linq;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _dbContext;
        private readonly IPhotoProcessing _photoProcessing;
        private readonly ISaveDataDb _saveDataDb;
        public EventService(AppDbContext appDbContext, IPhotoProcessing photoProcessing, ISaveDataDb saveDataDb)
        {
            _photoProcessing = photoProcessing;
            _dbContext = appDbContext;
            _saveDataDb = saveDataDb;
        }

        public async Task AddIndividualMathDto(IndividualMatchDto individualMatch)
        {
            var eventId = await _dbContext.Events
            .AsNoTracking()
            .Where(e => e.NameEvent == individualMatch.NameEvent)
            .Select(e => e.IdEvent)
            .FirstOrDefaultAsync();
            if (eventId == null) throw new NotFoundException("Event not found");

            _dbContext.Locations.Add(new Location{
                LocationName = individualMatch.location.address,
                Latitude = individualMatch.location.lat,
                Longitude = individualMatch.location.lng,
            });

            var entity = new IndividualMatch
            {
                IdEvent = eventId,
                loginFirstAthlete = individualMatch.loginFirstAthlete,
                loginSecondAthlete = individualMatch.loginSecondAthlete,
                DataMatch = individualMatch.DateStart,
                TimeMatch = individualMatch.TimeStart,
                LocationName = individualMatch.location.address,
                Tour = individualMatch.tour,
                AddInformation = individualMatch.AddInformation
            };
            _dbContext.IndividualMatches.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateEvent(EventDto eventDto)
        {
            if (await _dbContext.Events
            .FirstOrDefaultAsync(e => e.NameEvent == eventDto.NameEvent) != null)
                throw new ValidationException("Team with this name exists");

            string photoPath = null;
            if (eventDto.EventPhoto != null)
            {
                photoPath = await _photoProcessing.SavePhotoAsync(eventDto.EventPhoto);
            }

            var jsonObJuserPhoto = JsonSerializer.Serialize(new UserPhoto
            {
                login = eventDto.NameEvent,
                ProfilePhoto = photoPath
            });
            _ = _saveDataDb.SaveDataToDb(jsonObJuserPhoto, "user-photo");

            var entity = new Event
            {
                NameEvent = eventDto.NameEvent,
                systems = eventDto.systems,
                EventPhoto = photoPath,
                DataStart = eventDto.DataStart.ToUniversalTime(),
                DataEnd = eventDto.DataEnd.ToUniversalTime(),
                description = eventDto.description
            };
            _dbContext.Events.Add(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}