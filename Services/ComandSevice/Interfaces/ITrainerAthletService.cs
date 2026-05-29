using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface ITrainerAthletService
    {
        Task<TrainerProfileDto> GetTrainerProfileAsync(string login);
        Task<List<TrainerTeamDto>> GetTrainerTeamsAsync(string login);

        Task AddAthletsToTrainer();
        Task ChangeStatusathlet();
        Task RemoveAthelte();
    }
}
