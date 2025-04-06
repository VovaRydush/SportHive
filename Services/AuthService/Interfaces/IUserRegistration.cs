 using DB.SportHive.Domain;

 namespace SportHive.Services.Interfaces
 {
 public interface IUserRegistration
    {
        Task Registration(UserInfoDto user);
        Task VeryfyEmail(UserVerificationDto info);
    }
 }