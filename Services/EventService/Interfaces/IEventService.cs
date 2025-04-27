using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
 public interface IEventService
 { 
     Task CreateEvent(EventDto eventDto);
     Task AddIndividualMathDto(TIMatchDto individualMatch);
     Task AddTeamMatch(TIMatchDto teamMatch);
     Task AddExtremeMathes(ExtreameMatchesDto extreameMatch);
     Task SaveExtreameAtheltes(List<string> athletes,long IdExtremeMatches);
     Task<long> GetEventId(string NameTeam);
     Task SaveLocation(LocationDto location);
     
 }
}