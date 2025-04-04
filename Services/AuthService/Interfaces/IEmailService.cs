namespace SportHive.Services.Interfaces
{
 public interface IEmailService
 { 
     Task SendEmailConfirmed(string email, string code);
 }
}