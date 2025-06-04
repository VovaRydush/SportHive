using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface IProfileInfo
    {
        Task SportInfo(AthletesTeamDto athlet);
    }
}