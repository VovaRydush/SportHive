using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Exceptions;

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

        public async Task AddExtremeMathes(List<ExtremeMatch> extreameMatches)
        {
            _dbContext.ExtremeMatches.AddRange(extreameMatches);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddIndividualMath(List<IndividualMatch> individualMatches)
        {
            _dbContext.IndividualMatches.AddRange(individualMatches);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddTeamMatch(List<TeamMatch> teamMatches)
        {
            _dbContext.TeamMatches.AddRange(teamMatches);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateEvent(EventDto eventDto)
        {
            if (await _dbContext.Events
            .FirstOrDefaultAsync(e => e.NameEvent == eventDto.NameEvent) != null)
                throw new ValidationException("Team with this name exists");

            string photoPath = "";
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
                TypeSport = eventDto.TypeSport,
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

        public async Task SaveExtreameAtheltes(List<string> athletes, long IdExtremeMatches)
        {
            var eMatchId = await _dbContext.ExtremeMatches
                .AsNoTracking()
                .Where(eMatch => eMatch.IdExtremeMatches == IdExtremeMatches)
                .Select(e => (long?)e.IdExtremeMatches).FirstOrDefaultAsync();

            if (eMatchId == null) throw new NotFoundException("Not found Match");

            var entities = new List<EMatchesAthlete>();
            foreach (var athlete in athletes)
            {
                var entity = new EMatchesAthlete
                {
                    IdExtremeMatches = eMatchId.Value,
                    loginAthlete = athlete
                };
                entities.Add(entity);
            }
            _dbContext.ExtremeMatchesAthetes.AddRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public void SaveLocation(AppDbContext appDbContext,LocationDto location)
        {
            appDbContext.Locations.Add(new Location
            {
                LocationName = location.address,
                Latitude = location.lat,
                Longitude = location.lng,
            });
        }
    }
}
