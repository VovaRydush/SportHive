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

        public UserRegistration(
            ICompliteUserProfile userProfile,
            ISaveDataDb saveDataDb,
            AppDbContext context,
            IRedisService database,
            IEmailService emailService,
            IPhotoProcessing photo)
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
            entity.dateBirhsday = DateTime.SpecifyKind(entity.dateBirhsday, DateTimeKind.Utc);

            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.login == entity.Login)
                .Select(u => new { u.login, u.Role })
                .FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException("Not Found User");

            string? photoPath = null;

            if (entity.ProfilePhoto != null)
                photoPath = await _photoprocessing.SavePhotoAsync(entity.ProfilePhoto);

            await UpsertUserPhoto(user.login, photoPath);

            switch (user.Role)
            {
                case "Athlete":
                    if (!await _context.Set<Athlete>().AnyAsync(x => x.login == user.login))
                    {
                        await _context.Set<Athlete>().AddAsync(new Athlete
                        {
                            login = user.login,
                            FirsName = entity.FistName,
                            LastName = entity.LastName,
                            DataBirth = entity.dateBirhsday,
                            TypeSport = entity.TypeSport
                        });
                    }
                    break;

                case "Trainer":
                    if (!await _context.Set<Trainer>().AnyAsync(x => x.login == user.login))
                    {
                        await _context.Set<Trainer>().AddAsync(new Trainer
                        {
                            login = user.login,
                            FirsName = entity.FistName,
                            LastName = entity.LastName
                        });
                    }
                    break;

                case "Judge":
                    if (!await _context.Set<Judge>().AnyAsync(x => x.login == user.login))
                    {
                        await _context.Set<Judge>().AddAsync(new Judge
                        {
                            login = user.login,
                            FirsName = entity.FistName,
                            LastName = entity.LastName
                        });
                    }
                    break;

                default:
                    throw new NotFoundException("Unknown role");
            }

            await _context.SaveChangesAsync();

            await _userProfile.CreateProfileInMongoAsync(
                entity.FistName + " " + entity.LastName,
                entity.Login,
                entity.TypeSport,
                entity.dateBirhsday);
        }

        public async Task ComplitePrifileOrganization(OrganizationInfoDto entity)
        {
            var user = await _context.Users
                .Where(u => u.Email == entity.Email || u.login == entity.Email)
                .Select(u => new { u.login, u.Email, u.Role })
                .FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException("User not found");

            if (user.Role != "Organization")
                throw new Exception("Поточний користувач не є організацією.");

            string? photoPath = null;

            if (entity.ProfilePhoto != null)
                photoPath = await _photoprocessing.SavePhotoAsync(entity.ProfilePhoto);

            await UpsertUserPhoto(user.login, photoPath);

            var organization = await _context.Set<Organization>()
                .FirstOrDefaultAsync(o => o.login == user.login);

            if (organization == null)
            {
                organization = new Organization
                {
                    login = user.login,
                    TypeOrganozation = entity.TypeOrganozation,
                    NameOrganization = entity.NameOrganization,
                    Country = entity.Country,
                    Description = entity.Description
                };

                await _context.Set<Organization>().AddAsync(organization);
            }
            else
            {
                organization.TypeOrganozation = entity.TypeOrganozation;
                organization.NameOrganization = entity.NameOrganization;
                organization.Country = entity.Country;
                organization.Description = entity.Description;
            }

            await _context.SaveChangesAsync();
        }

        public async Task LinkOrganizationJudge(OrganizationJudgeDto entity)
        {
            var organization = await _context.Users
                .AsNoTracking()
                .Where(e => e.login == entity.LoginOrganization)
                .Select(e => new { e.login })
                .FirstOrDefaultAsync();

            var user = await _context.Users
                .AsNoTracking()
                .Where(e => e.login == entity.LoginEntyty)
                .Select(e => new { e.login })
                .FirstOrDefaultAsync();

            if (organization == null || user == null)
                throw new NotFoundException("User or organization not found");

            if (entity.Role == "Judge")
            {
                var exists = await _context.OrginizationJudges
                    .AnyAsync(x => x.LoginOrganization == organization.login && x.LoginJudge == user.login);

                if (!exists)
                {
                    _context.OrginizationJudges.Add(new OrganizationJudge
                    {
                        LoginOrganization = organization.login,
                        LoginJudge = user.login
                    });
                }
            }

            if (entity.Role == "Trainer")
            {
                var exists = await _context.OrganizationTrainers
                    .AnyAsync(x => x.LoginOrganization == organization.login && x.LoginTraine == user.login);

                if (!exists)
                {
                    _context.OrganizationTrainers.Add(new OrganizationTrainer
                    {
                        LoginOrganization = organization.login,
                        LoginTraine = user.login
                    });
                }
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

            if (existingUser != null)
                throw new Exception("Користувач з таким email або логіном вже існує.");

            var user = new User
            {
                Email = entity.Email,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password),
                isEmailConfirmed = false,
                Role = entity.Role,
                login = entity.Login,
                refreshToken = Guid.NewGuid().ToString()
            };

            await _context.Users.AddAsync(user);

            if (entity.Role == "Organization")
            {
                var organizationExists = await _context.Set<Organization>()
                    .AnyAsync(o => o.login == entity.Login);

                if (!organizationExists)
                {
                    await _context.Set<Organization>().AddAsync(new Organization
                    {
                        login = entity.Login,
                        NameOrganization = entity.Login,
                        TypeOrganozation = "Organization",
                        Description = "Профіль організації ще не заповнено.",
                        Country = ""
                    });
                }
            }

            await _context.SaveChangesAsync();

            await _emailService.SendEmail(new EmailMessageDto
            {
                From = "vadimrudis7@gmail.com",
                To = entity.Email,
                Subject = "Підтвердження email",
                Body = null
            });
        }

        public async Task VeryfyEmail(UserVerificationDto info)
        {
            var veryfyCode = await _redis.GetEntity(info.Email);

            if (info.Code == veryfyCode)
            {
                var email = await _context.Users.FirstOrDefaultAsync(u => u.Email == info.Email);

                if (email != null)
                {
                    await _redis.DeleteVerifacionCode(info.Email);
                    email.isEmailConfirmed = true;
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                throw new NotFoundException("Код не правельний!");
            }
        }

        private async Task UpsertUserPhoto(string login, string? photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
                return;

            var userPhoto = await _context.Set<UserPhoto>()
                .FirstOrDefaultAsync(x => x.login == login);

            if (userPhoto == null)
            {
                await _context.Set<UserPhoto>().AddAsync(new UserPhoto
                {
                    login = login,
                    ProfilePhoto = photoPath
                });
            }
            else
            {
                userPhoto.ProfilePhoto = photoPath;
            }
        }
    }
}
