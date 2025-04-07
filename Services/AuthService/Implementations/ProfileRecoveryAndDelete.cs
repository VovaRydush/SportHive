using System.Net.Mail;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class ProfileManipulete : IProfileManipulete
    {
        private readonly AppDbContext _dbcontext;
        private readonly IEmailService _emailService;
        private readonly IRedisService _redisService;
        
        public ProfileManipulete(AppDbContext dbContext, IEmailService emailService,IRedisService redisService)
        {
            _redisService = redisService;
            _dbcontext = dbContext;
            _emailService = emailService;
        }

        public async Task PasswordRecovery(string Email,string newPassword)
        {
           var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == Email);
           user.HashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
           await _dbcontext.SaveChangesAsync();
        }

        public async Task SendVereficationCode(string Email)
        {
            Random random = new Random();
            string code = random.Next(100000, 1000000).ToString();
           await _emailService.SendEmail(new MailMessage("vadimrudis7@gmail.com", Email)
            {
                Subject = "Код відновлення:",
                Body = $"Ваш код: {code} для відновлення паролю.",
                IsBodyHtml = true
            });
            await _redisService.SetVerifacionCode(Email,code);
        }
    }
}