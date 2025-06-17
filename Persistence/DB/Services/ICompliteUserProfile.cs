namespace SportHive.Services.Interfaces
{
    public interface ICompliteUserProfile
    {
        Task CreateProfileInMongoAsync(string fullName, string Login, string sportType,DateTime dateBirhsday);
    }
}