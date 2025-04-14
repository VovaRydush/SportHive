using System.Net.Mail;
using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Exceptions;
using SportHive.Services.Interfaces;


namespace SportHive.Implementations
{
    public class UserRegistration : IUserRegistration
    {
        private readonly AppDbContext _context;
        private readonly IRedisService _redis;
        private readonly IEmailService _emailService;
        private readonly IPhotoProcessing _photoprocessing;
        public UserRegistration(AppDbContext context, IRedisService database, IEmailService emailService, IPhotoProcessing photo)
        {
            _photoprocessing = photo;
            _emailService = emailService;
            _redis = database;
            _context = context;
        }
        public async Task ComplitePrifile(RoleInfoDto entity)
        {
            var user = await _context.Users
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(u => u.login == entity.Login);

            if (user == null)
                throw new NotFoundException("Not Found");

            string photoPath = null;
            if (entity.ProfilePhoto != null)
            {
                photoPath = await _photoprocessing.SavePhotoAsync(entity.ProfilePhoto);
            }
            _context.UserPhotos.Add(new UserPhoto
            {
                login = user.login,
                ProfilePhoto = photoPath
            });
            switch (user.Role)
            {
                case "Athlete":
                    _context.Athletes.Add(new Athlete
                    {
                        login = user.login,
                        FirsName = entity.FistName,
                        LastName = entity.LastName,
                        TypeSport = entity.TypeSport
                    });
                    break;

                case "Trainer":
                    _context.Trainers.Add(new Trainer
                    {
                        login = user.login,
                        FirsName = entity.FistName,
                        LastName = entity.LastName,
                    });
                    break;

                case "Judge":
                    _context.Judges.Add(new Judge
                    {
                        login = user.login,
                        FirsName = entity.FistName,
                        LastName = entity.LastName,
                    });
                    break;

                default:
                    throw new NotFoundException("Unknown role");
            }

            await _context.SaveChangesAsync();
        }

        public async Task ComplitePrifileOrganization(OrganizationInfoDto entity)
        {
            var user = await _context.Users
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(u => u.Email == entity.Email);
            if (user == null)
                throw new NotFoundException("User not found");

            string photoPath = null;
            if (entity.ProfilePhoto != null)
            {
                photoPath = await _photoprocessing.SavePhotoAsync(entity.ProfilePhoto);
            }
            _context.UserPhotos.Add(new UserPhoto
            {
                login = user.login,
                ProfilePhoto = photoPath
            });
            _context.Organizations.Add(new Organization
            {
                login = user.login,
                TypeOrganozation = entity.TypeOrganozation,
                NameOrganization = entity.NameOrganization,
                Country = entity.Country,
                Description = entity.Description
            });
            await _context.SaveChangesAsync();
        }

        public async Task LinkOrganizationJudge(OrganizationJudgeDto entity)
        {
            var organization = await _context.Users.FirstAsync(e => e.login == entity.LoginOrganization);
            var Entity = await _context.Users.FirstAsync(e => e.login == entity.LoginEntyty);
            if (entity.Role == "Judge")
            {
                var organizationJudge = new OrganizationJudge
                {
                    LoginOrganization = organization.login,
                    LoginJudge = Entity.login
                };
                _context.OrginizationJudges.Add(organizationJudge);
            }
            if (entity.Role == "Trainer")
            {
                var organizationTrainer = new OrganizationTrainer
                {
                    LoginOrganization = organization.login,
                    LoginTraine = Entity.login
                };
                _context.OrganizationTrainers.Add(organizationTrainer);
            }
            await _context.SaveChangesAsync();
        }

        public async Task Registration(UserInfoDto entity)
        {
            var existingUser = await _context
                                    .Users
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(u => u.Email == entity.Email || u.login == entity.Login);

            if (existingUser != null) throw new Exception("Користувач з таким email або логіном вже існує.");

            Random random = new Random();
            string code = random.Next(100000, 1000000).ToString();

            var user = new User
            {
                Email = entity.Email,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password),
                isEmailConfirmed = false,
                Role = entity.Role,
                login = entity.Login,
                refreshToken = Guid.NewGuid().ToString()
            };

            _context.Users.Add(user);
            

            await _context.SaveChangesAsync();
            
            _redis.SetVerifacionCode(entity.Email, code);

            await _emailService.SendEmail(
                 new MailMessage("vadimrudis7@gmail.com", entity.Email)
                 {
                     Subject = "Підтвердження email",
                     Body = $"Ваш код: {code} для підтвердження email.",
                     IsBodyHtml = true
                 });
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
            else throw new NotFoundException("Код не правельний!");
        }

    }

}