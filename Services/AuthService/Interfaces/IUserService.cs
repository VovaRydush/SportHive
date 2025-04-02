 using DB.SportHive.Domain;
using DB.SportHive.Persistence;

 namespace SportHive.Services.Interfaces
 {
 public interface IUserService
    {
        Task Registration(string email, string password);
        Task<List<User>> GetAllUsers();
        Task VeryfyEmail(string tempToken);
    }
 }