using SportHive.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace SportHive.Implementations
{
    class EmailService : IEmailService
    {
        public async Task SendEmailConfirmed(string email, string code)
        {

            var message = new MailMessage("vadimrudis7@gmail.com", email)
            {
                Subject = "Підтвердження email",
                Body = $"Ваш код: {code} для підтвердження email.",
                IsBodyHtml = true
            };

            try
            {
                using var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Credentials = new NetworkCredential("vadimrudis7@gmail.com", "qtfo apob lfjd nqfp"),
                    EnableSsl = true,
                    Port = 587
                };

                await smtp.SendMailAsync(message);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }

        }
    }
}