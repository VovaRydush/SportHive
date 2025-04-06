using System.Net.Mail;
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
                                     .FirstOrDefaultAsync(u => u.Email == entity.Email);

            if (user == null)
                throw new Exception("User not found");

            string photoPath = null;
            if (entity.ProfilePhoto != null)
            {
                photoPath = await _photoprocessing.SavePhotoAsync(entity.ProfilePhoto);
            }
            _context.UserPhotos.Add(new UserPhoto
            {
                Id = user.Id,
                ProfilePhoto = photoPath
            });
            switch (user.Role)
            {
                case "Athlete":
                    _context.Athletes.Add(new Athlete
                    {
                        Id = user.Id,
                        FirsName = entity.FistName,
                        LastName = entity.LastName,
                        TypeSport = entity.TypeSport
                    });
                    break;

                case "Trainer":
                    _context.Trainers.Add(new Trainer
                    {
                        Id = user.Id,
                        FirsName = entity.FistName,
                        LastName = entity.LastName,
                    });
                    break;

                case "Judge":
                    _context.Judges.Add(new Judge
                    {
                        Id = user.Id,
                        FirsName = entity.FistName,
                        LastName = entity.LastName,
                    });
                    break;

                default:
                    throw new Exception("Unknown role");
            }

            await _context.SaveChangesAsync();
        }

        public async Task ComplitePrifileOrganization(OrganizationInfoDto entity)
        {
            var user = await _context.Users
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(u => u.Email == entity.Email);
            if (user == null)
                throw new Exception("User not found");

            string photoPath = null;
            if (entity.ProfilePhoto != null)
            {
                photoPath = await _photoprocessing.SavePhotoAsync(entity.ProfilePhoto);
            }
            _context.UserPhotos.Add(new UserPhoto
            {
                Id = user.Id,
                ProfilePhoto = photoPath
            });
            _context.Organizations.Add(new Organization
            {
                Id = user.Id,
                TypeOrganozation = entity.TypeOrganozation,
                NameOrganization = entity.NameOrganization,
                Description = entity.Description
            });
            await _context.SaveChangesAsync();
        }

        public async Task Registration(UserInfoDto entity)
        {
            var existingUser = await _context
                                    .Users
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(u => u.Email == entity.Email);

            if (existingUser != null) throw new Exception("Користувач з таким email вже існує.");

            Random random = new Random();
            string code = random.Next(100000, 1000000).ToString();
            using (var context = _context)
            {
                var user = new User
                {
                    Email = entity.Email,
                    HashPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password),
                    isEmailConfirmed = false,
                    Role = entity.Role,
                    refreshToken = Guid.NewGuid().ToString()
                };

                context.Users.Add(user);
                await _context.SaveChangesAsync();
                await _redis.SetVerifacionCode(entity.Email, code);
                
                await _emailService.SendEmail(
                     new MailMessage("vadimrudis7@gmail.com", entity.Email)
                     {
                         Subject = "Підтвердження email",
                         Body = $"Ваш код: {code} для підтвердження email.",
                         IsBodyHtml = true
                     });
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
                else throw new Exception("Щось пішло не так!");
            }
            else throw new Exception("Код не правельний!");
        }

    }

}