using DB.SportHive.Domain;
using DB.SportHive.Persistence;

namespace SportHive.Services.Interfaces
{
    public interface IProfileInfo
    {
        Task SportInfo(AthletesTeamDto athlet);
    }
}