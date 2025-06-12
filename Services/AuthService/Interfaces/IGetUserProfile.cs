using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface IGetUserProfile
    {
        Task<string> GetUserPhoto(string loginUser);
        Task<AthleteProfile> GetAllInfoUser(string loginUser);
    }
}