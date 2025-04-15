using System.Net.Mail;
using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
 public interface IEmailService
 { 
     Task SendEmail(EmailMessageDto message);
 }
}