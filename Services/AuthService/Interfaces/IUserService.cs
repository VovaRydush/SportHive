 using DB.SportHive.Domain;

 namespace SportHive.Services.Interfaces
 {
 public interface IUserService
    {
        Task Registration(string email, string password);
        Task<List<User>> GetAllUsers();
        Task VeryfyEmail(UserVerificationDto info);
    }
 }