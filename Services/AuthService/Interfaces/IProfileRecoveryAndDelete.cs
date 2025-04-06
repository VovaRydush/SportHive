namespace SportHive.Services.Interfaces
{
 public interface IProfileManipulete
 {
    Task PasswordRecovery(string Email,string newPassword);
    Task SendVereficationCode(string Email);
    Task DeleteProfile(string Email);
 }
}