using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Services.Interfaces;


namespace SportHive.Implementations
{
    public class UserRegistration : IUserRegistration
    {
        private readonly AppDbContext _context;
        private readonly IRedisService _redis;
        private readonly IEmailService _emailService;
        public UserRegistration(AppDbContext context, IRedisService database,IEmailService emailService)
        {
            _emailService =  emailService;
            _redis = database;
            _context = context;
        }

        public async Task Registration(string email, string password)
        {
            var existingUser = await _context
                                    .Users
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(u => u.Email == email);

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
                    refreshToken = Guid.NewGuid().ToString()
                };

                await _redis.SetVerifacionCode(email, code);
                context.Users.Add(user);
                await _context.SaveChangesAsync();
                await _emailService.SendEmailConfirmed(email, code);
            }
        }

        public async Task VeryfyEmail(UserVerificationDto info)
        {
            string veryfyCode = await _redis.GetVerifacionCode(info.Email);
            if (info.Code == veryfyCode)
            {
                var Email = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == info.Email);
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