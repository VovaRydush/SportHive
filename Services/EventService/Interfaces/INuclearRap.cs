using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface INuclearRap
    {
        Task ChangePosition(AthleteValuesDto position);
        Task<long> CountMatchs(string loginPlayer);
        Task<long> CountWinIndividual(string loginPlayer);
        Task<long> CountLossIndividual(string loginPlayer);
        Task<long> CountDrawIndividual(string loginPlayer);
        Task<long> CountWinTeam(AthletesTeamDto athlet);
        Task<long> CountLossTeam(AthletesTeamDto athlet);
        Task SetWeightCategory(AthleteValuesDto newWeight);
    }
}