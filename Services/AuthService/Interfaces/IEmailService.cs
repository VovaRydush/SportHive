using System.Net.Mail;

namespace SportHive.Services.Interfaces
{
 public interface IEmailService
 { 
     Task SendEmail(MailMessage message);
 }
}