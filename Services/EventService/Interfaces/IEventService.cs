using DB.SportHive.Domain;
using DB.SportHive.Persistence;

namespace SportHive.Services.Interfaces
{
 public interface IEventService
 { 
     Task CreateEvent(EventDto eventDto);
     Task AddIndividualMath(List<IndividualMatch> individualMatches);
     Task AddTeamMatch(List<TeamMatch> teamMatches);
     Task AddExtremeMathes(List<ExtremeMatch> extreameMatches);
     Task SaveExtreameAtheltes(List<string> athletes,long IdExtremeMatches);
     Task<long> GetEventId(string NameTeam);
     void SaveLocation(AppDbContext appDbContext,LocationDto location);
     
 }
}