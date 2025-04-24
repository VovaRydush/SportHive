using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
 public interface IEventService
 { 
     Task CreateEvent(EventDto eventDto);
     Task AddIndividualMathDto(IndividualMatchDto individualMatch);
 }
}