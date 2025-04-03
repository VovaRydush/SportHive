using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SportHive.Services.Interfaces;
using StackExchange.Redis;
using System.Net;
using System.Net.Mail;


namespace SportHive.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IRedisService _redis;
        public UserService(AppDbContext context, IRedisService database)
        {
            _redis = database;
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
            Random random = new Random();
            string code = random.Next(100000, 1000000).ToString();
            using (var context = _context)
            {
                var user = new User
                {
                    Email = email,
                    HashPassword = BCrypt.Net.BCrypt.HashPassword(password),
                    isEmailConfirmed = false,
                    tempToken = Guid.NewGuid().ToString()
                };

                await _redis.SetVerifacionCode(email, code);
                context.Users.Add(user);
                await _context.SaveChangesAsync();
                await SendEmailConfirmed(email, code);
            }
        }
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

        public async Task VeryfyEmail(UserVerificationDto info)
        {
            string veryfyCode = await _redis.GetVerifacionCode(info.Email);
            if (info.Code == veryfyCode)
            {
                var Email = await _context.Users.FirstOrDefaultAsync(u => u.Email == info.Email);
                if (Email != null)
                {
                    await _redis.DeleteVerifacionCode(info.Email);
                    Email.isEmailConfirmed = true;
                    await _context.SaveChangesAsync();
                }
            }
            else throw new Exception("Код не правельний!");
        }

    }
}