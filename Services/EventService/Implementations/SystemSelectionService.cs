using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Exceptions;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SystemSelectionService : ISystemSelectionService
    {
        private readonly IEventService _eventService;
        private readonly AppDbContext _appDbContext;
        public SystemSelectionService(IEventService eventService, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _eventService = eventService;
        }
        public async Task CreateMatchWithSSystem(MatchsAbstractionDto teamComposition)
        {
            var IdEvent = _appDbContext.Events
                .AsNoTracking()
                .Where(e => e.NameEvent == teamComposition.NameEvent)
                .Select(ev => ev.IdEvent)
                .FirstOrDefaultAsync();
            if (IdEvent == null) throw new NotFoundException("Not Found Event");

            switch (teamComposition.system)
            {
                case "DoubleElimination":
                    await DoubleElimination(teamComposition);
                    break;
                case "Group":
                    await Group(teamComposition);
                    break;
                case "Knockout":
                    await Knockout(teamComposition);
                    break;
                case "Mixsed":
                    await Mixsed(teamComposition);
                    break;
                case "Olympic":
                    await Olympic(teamComposition);
                    break;
                case "PlayOff":
                    await PlayOff(teamComposition);
                    break;
                case "QualificationByStandards":
                    await QualificationByStandardsDorobitiPererobiti(teamComposition);
                    break;
                case "RoundRobin":
                    await RoundRobin(teamComposition);
                    break;
                case "SwissSystem":
                    await SwissSystem(teamComposition);
                    break;
            }
        }

        public Task DoubleElimination(MatchsAbstractionDto match)
        {
            throw new NotImplementedException();
        }

        public Task Group(MatchsAbstractionDto match)
        {
            throw new NotImplementedException();
        }

        public Task Knockout(MatchsAbstractionDto match)
        {
            throw new NotImplementedException();
        }

        public Task Mixsed(MatchsAbstractionDto match)
        {
            throw new NotImplementedException();
        }

        public Task Olympic(MatchsAbstractionDto match)
        {
            throw new NotImplementedException();
        }

        public Task PlayOff(MatchsAbstractionDto match)
        {
            throw new NotImplementedException();
        }

        public Task QualificationByStandardsDorobitiPererobiti(MatchsAbstractionDto match)
        {
            throw new NotImplementedException();
        }

        public Task RoundRobin(MatchsAbstractionDto match)
        {
            throw new NotImplementedException();
        }

        public Task SwissSystem(MatchsAbstractionDto match)
        {
            throw new NotImplementedException();
        }
    }
}