using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface IEnterFouls
    {
        Task SetFoul(FoulDto foul);
    }
}