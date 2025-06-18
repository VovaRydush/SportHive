using DB.SportHive.Domain;

public interface IAthleteService
{
    Task<List<AthleteSearchResultDto>> SearchAthletesAsync(string searchTerm);
}