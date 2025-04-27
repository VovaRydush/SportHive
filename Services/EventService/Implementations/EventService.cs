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

        public async Task AddExtremeMathes(ExtreameMatchesDto extreameMatch)
        {
            await SaveLocation(extreameMatch.location);
            var entity =  new TeamMatch
            {
                IdEvent = await GetEventId(extreameMatch.NameEvent),
                DataMatch = extreameMatch.DataMatch,
                TimeMatch = extreameMatch.TimeMatch,
                Tour = extreameMatch.tour,
                AddInformation = extreameMatch.AddInformation
            };
            _dbContext.TeamMatches.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddIndividualMathDto(TIMatchDto individualMatch)
        {

            await SaveLocation(individualMatch.location);

            var entity = new IndividualMatch
            {
                IdEvent = await GetEventId(individualMatch.NameEvent),
                loginFirstAthlete = individualMatch.FirstEntity,
                loginSecondAthlete = individualMatch.SecondEntity,
                DataMatch = individualMatch.DateStart,
                TimeMatch = individualMatch.TimeStart,
                LocationName = individualMatch.location.address,
                Tour = individualMatch.tour,
                AddInformation = individualMatch.AddInformation
            };

            _dbContext.IndividualMatches.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddTeamMatch(TIMatchDto teamMatch)
        {
            await SaveLocation(teamMatch.location);

            var Karina_Sadik_Temki = new TeamMatch
            {
                IdEvent = await GetEventId(teamMatch.NameEvent),
                NameFirstTeam = teamMatch.FirstEntity,
                NameSecondTeam = teamMatch.SecondEntity,
                DataMatch = teamMatch.DateStart,
                TimeMatch = teamMatch.TimeStart,
                Tour = teamMatch.tour,
                AddInformation = teamMatch.AddInformation
            };
            _dbContext.TeamMatches.Add(Karina_Sadik_Temki);
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

        public async Task<long> GetEventId(string NameTeam)
        {
            var eventId = await _dbContext.Events
            .AsNoTracking()
            .Where(e => e.NameEvent == NameTeam)
            .Select(e => e.IdEvent)
            .FirstOrDefaultAsync();

            if (eventId == null) throw new NotFoundException("Event not found");
            return eventId;
        }

        public Task SaveExtreameAtheltes(List<string> athletes,long IdExtremeMatches)
        {
            throw new Exception();
        }

        public async Task SaveLocation(LocationDto location)
        {
            _dbContext.Locations.Add(new Location
            {
                LocationName = location.address,
                Latitude = location.lat,
                Longitude = location.lng,
            });
        }
    }
}