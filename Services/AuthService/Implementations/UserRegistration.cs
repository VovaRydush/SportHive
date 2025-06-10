using System.Net.Mail;
using System.Text.Json;
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
        private readonly ISaveDataDb _saveDataDb;
        private readonly IPhotoProcessing _photoprocessing;
        private readonly ICompliteUserProfile _userProfile;
        public UserRegistration(ICompliteUserProfile userProfile,ISaveDataDb saveDataDb, AppDbContext context, IRedisService database, IEmailService emailService, IPhotoProcessing photo)
        {
            _userProfile = userProfile;
            _saveDataDb = saveDataDb;
            _photoprocessing = photo;
            _emailService = emailService;
            _redis = database;
            _context = context;
        }
        public async Task ComplitePrifile(RoleInfoDto entity)
        {
            var user = await _context.Users
                                    .AsNoTracking()
                                    .Where(u => u.login == entity.Login)
                                    .Select(u => new { u.login, u.Role })
                                    .FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException("Not Found");

            string photoPath = null;
            if (entity.ProfilePhoto != null)
            {
                photoPath = await _photoprocessing.SavePhotoAsync(entity.ProfilePhoto);
            }

            var jsonObJuserPhoto = JsonSerializer.Serialize(new UserPhoto
            {
                login = user.login,
                ProfilePhoto = photoPath
            });
            _ = _saveDataDb.SaveDataToDb(jsonObJuserPhoto, "user-photo");
            switch (user.Role)
            {
                case "Athlete":
                    var jsonAthlet = JsonSerializer.Serialize(new Athlete
                    {
                        login = user.login,
                        FirsName = entity.FistName,
                        LastName = entity.LastName,
                        TypeSport = entity.TypeSport
                    });
                    _ = _saveDataDb.SaveDataToDb(jsonAthlet, "user-athlete");
                    break;

                case "Trainer":
                    var jsonTrainer = JsonSerializer.Serialize(new Trainer
                    {
                        login = user.login,
                        FirsName = entity.FistName,
                        LastName = entity.LastName,
                    });
                    _ = _saveDataDb.SaveDataToDb(jsonTrainer, "user-trainer");
                    break;

                case "Judge":
                    var jsonJudge = JsonSerializer.Serialize(new Judge
                    {
                        login = user.login,
                        FirsName = entity.FistName,
                        LastName = entity.LastName,
                    });
                    _ = _saveDataDb.SaveDataToDb(jsonJudge, "user-judge");
                    break;

                default:
                    throw new NotFoundException("Unknown role");
            }
            await _userProfile.CreateProfileInMongoAsync(entity.FistName+" "+entity.LastName,entity.Login,entity.TypeSport);
        }

        public async Task ComplitePrifileOrganization(OrganizationInfoDto entity)
        {
            var user = await _context.Users
                                     .AsNoTracking()
                                     .Where(u => u.Email == entity.Email)
                                     .Select(u => new { u.login })
                                     .FirstOrDefaultAsync();
            if (user == null)
                throw new NotFoundException("User not found");

            string photoPath = null;
            if (entity.ProfilePhoto != null)
            {
                photoPath = await _photoprocessing.SavePhotoAsync(entity.ProfilePhoto);
            }
            var jsonObJuserPhoto = JsonSerializer.Serialize(new UserPhoto
            {
                login = user.login,
                ProfilePhoto = photoPath
            });
            _ = _saveDataDb.SaveDataToDb(jsonObJuserPhoto, "user-photo");

            var jsonOrganization = JsonSerializer.Serialize(new Organization
            {
                login = user.login,
                TypeOrganozation = entity.TypeOrganozation,
                NameOrganization = entity.NameOrganization,
                Country = entity.Country,
                Description = entity.Description
            });
            _ = _saveDataDb.SaveDataToDb(jsonOrganization, "user-organization");
        }

        public async Task LinkOrganizationJudge(OrganizationJudgeDto entity)
        {
            var organization = await _context.Users
                                                .AsNoTracking()
                                                .Where(e => e.login == entity.LoginOrganization)
                                                .Select(e => new { e.login })
                                                .FirstOrDefaultAsync();

            var Entity = await _context.Users
                                        .AsNoTracking()
                                        .Where(e => e.login == entity.LoginEntyty)
                                        .Select(e => new {e.login})
                                        .FirstOrDefaultAsync();

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
            var existingUser = await _context.Users
                                    .AsNoTracking()
                                    .Where(u => u.Email == entity.Email || u.login == entity.Login)
                                    .Select(u => new { u.login })
                                    .FirstOrDefaultAsync();

            if (existingUser != null) throw new Exception("Користувач з таким email або логіном вже існує.");

            var user = new User
            {
                Email = entity.Email,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password),
                isEmailConfirmed = false,
                Role = entity.Role,
                login = entity.Login,
                refreshToken = Guid.NewGuid().ToString()
            };

            var jsonObj = JsonSerializer.Serialize(user);

            _ = _saveDataDb.SaveDataToDb(jsonObj, "user_regist");

            _ = _emailService.SendEmail(new EmailMessageDto
            {
                From = "vadimrudis7@gmail.com",
                To = entity.Email,
                Subject = "Підтвердження email",
                Body = "Ваш код: ",
            });

        }

        public async Task VeryfyEmail(UserVerificationDto info)
        {
            string veryfyCode = await _redis.GetEntity(info.Email);
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