using System.Net.Mail;
using DB.SportHive.Domain;
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

        public ProfileManipulete(AppDbContext dbContext, IEmailService emailService, IRedisService redisService)
        {
            _redisService = redisService;
            _dbcontext = dbContext;
            _emailService = emailService;
        }

        public async Task PasswordRecovery(string login, string newPassword)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.login == login);
            user.HashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task SendVereficationCode(string Email)
        {
            await _emailService.SendEmail(new EmailMessageDto
            {
                From = "vadimrudis7@gmail.com",
                To = Email, 
                Subject = "Код відновлення:",
                Body = "Ваш код: "
            });
        }
    }
}