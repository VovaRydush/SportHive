using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Services.Interfaces;
using System.Net;
using System.Net.Mail;


namespace SportHive.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task Registration(string email, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                throw new Exception("Користувач з таким email вже існує.");
            }

            using (var context = _context)
            {
                var user = new User
                {
                    Email = email,
                    HashPassword = BCrypt.Net.BCrypt.HashPassword(password),
                    isEmailConfirmed = false,
                    tempToken = Guid.NewGuid().ToString()
                };

                context.Users.Add(user);
                await _context.SaveChangesAsync();
                await SendEmailConfirmed(email, user.tempToken);
            }
        }
        public async Task SendEmailConfirmed(string email, string refreshToken)
        {
            var verifyUrl = $"http://localhost:5154/verify?token={refreshToken}";
            var message = new MailMessage("vadimrudis7@gmail.com", email)
            {
                Subject = "Підтвердження email",
                Body = $"Натисніть <a href='{verifyUrl}'>тут</a> для підтвердження email.",
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

        public async Task VeryfyEmail(string tempToken)
        {
            var existToken = await _context.Users.FirstOrDefaultAsync(u => u.tempToken == tempToken);
            if (existToken != null)
            {
                existToken.isEmailConfirmed = true;
                await _context.SaveChangesAsync();
            }
            else throw new Exception("Токен не дійсний");
        }
    }
}