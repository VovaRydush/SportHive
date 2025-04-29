using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface ISystemSelectionService
    {
        Task CreateMatchWithSSystem(Matchs teamComposition);
    }
}