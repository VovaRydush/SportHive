using SportHive.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace SportHive.Implementations
{
    class EmailService : IEmailService
    {
        public async Task SendEmail(MailMessage message)
        {

            using var smtp = new SmtpClient("smtp.gmail.com")
            {
                Credentials = new NetworkCredential("vadimrudis7@gmail.com", "qtfo apob lfjd nqfp"),
                EnableSsl = true,
                Port = 587
            };
            await smtp.SendMailAsync(message);
        }
    }
}