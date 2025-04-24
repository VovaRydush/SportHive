using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
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