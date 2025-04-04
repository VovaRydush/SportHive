 using DB.SportHive.Domain;

 namespace SportHive.Services.Interfaces
 {
 public interface IUserRegistration
    {
        Task Registration(string email, string password);
        Task VeryfyEmail(UserVerificationDto info);
    }
 }