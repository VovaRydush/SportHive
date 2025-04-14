using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface ITrainerAthletService
    {
        Task AddAthletsToTrainer();
        Task ChangeStatusathlet();
        Task RemoveAthelte();
    }
}
