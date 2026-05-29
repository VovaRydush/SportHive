using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SportHive.Exceptions;
using SportHive.Services.Interfaces;
using System.Text.Json;

namespace SportHive.Implementations
{
    class TeamOperateService : ITeamOperateService
    {
        private readonly AppDbContext _context;
        private readonly IPhotoProcessing _photoProcessing;
        private readonly ISaveDataDb _saveDataDb;
        private readonly IRedisService _redisService;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;

        public TeamOperateService(
            AppDbContext context,
            IPhotoProcessing photoProcessing,
            ISaveDataDb saveDataDb,
            IMongoDbService mongoDbService,
            IRedisService redisService)
        {
            _context = context;
            _photoProcessing = photoProcessing;
            _saveDataDb = saveDataDb;
            _redisService = redisService;
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("AthleteProfile");
        }

        public async Task CreateTeamAsync(TeamModelDto team)
        {
            NormalizeTeamDto(team);

            var exists = await _context.Teams
                .AsNoTracking()
                .AnyAsync(t => t.TeamName == team.NameTeam);

            if (exists)
                throw new CustomAppException($"Команда з назвою '{team.NameTeam}' вже існує", 400, "Team already exists");

            await EnsureTrainerExists(team.LoginTrainer);
            await EnsureAthletesExist(team.Athlets.Select(a => a.LoginAthlets));

            string photoPath = string.Empty;
            if (team.Photo != null)
                photoPath = await _photoProcessing.SavePhotoAsync(team.Photo);

            var newTeam = new Team
            {
                TeamName = team.NameTeam,
                LoginTrainer = team.LoginTrainer,
                TypeSport = team.TypeSport,
                TeamPhoto = photoPath ?? string.Empty
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();

            _context.Teams.Add(newTeam);
            await _context.SaveChangesAsync();

            var athletes = BuildUniqueAthletes(team.NameTeam, team.Athlets);

            if (athletes.Count > 0)
            {
                _context.teamAthletes.AddRange(athletes);
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrWhiteSpace(photoPath))
            {
                var jsonObJuserPhoto = JsonSerializer.Serialize(new UserPhoto
                {
                    login = team.NameTeam,
                    ProfilePhoto = photoPath
                });

                _ = _saveDataDb.SaveDataToDb(jsonObJuserPhoto, "user-photo");
            }

            await transaction.CommitAsync();

            foreach (var athlete in athletes)
                await UpdateAthleteMongoSafe(athlete.loginAthlets, team.NameTeam, athlete.AthleteStatus);

            await ClearTeamCache(team.NameTeam);
        }

        public async Task AddAthletes(List<TeamAthleteDto> athleteDto)
        {
            if (athleteDto == null || athleteDto.Count == 0)
                throw new CustomAppException("Список спортсменів порожній", 400, "Empty athletes list");

            var nameTeam = athleteDto.First().NameTeam?.Trim();
            if (string.IsNullOrWhiteSpace(nameTeam))
                throw new CustomAppException("Не передано назву команди", 400, "NameTeam is required");

            await EnsureTeamExists(nameTeam);
            await EnsureAthletesExist(athleteDto.Select(a => a.LoginAthlets));

            var existingLogins = await _context.teamAthletes
                .AsNoTracking()
                .Where(ta => ta.NameTeam == nameTeam)
                .Select(ta => ta.loginAthlets)
                .ToListAsync();

            var existingSet = existingLogins.ToHashSet(StringComparer.OrdinalIgnoreCase);

            var entities = BuildUniqueAthletes(nameTeam, athleteDto)
                .Where(a => !existingSet.Contains(a.loginAthlets))
                .ToList();

            if (entities.Count == 0)
                return;

            _context.teamAthletes.AddRange(entities);
            await _context.SaveChangesAsync();

            foreach (var athlete in entities)
                await UpdateAthleteMongoSafe(athlete.loginAthlets, nameTeam, athlete.AthleteStatus);

            await ClearTeamCache(nameTeam);
        }

        public async Task ChangeStatusAthlete(NewSatatusAthlete newSatatus)
        {
            ValidateStatusRequest(newSatatus);

            var athlete = await GetAthlete(newSatatus.LoginAthlete, newSatatus.NameTeam);
            athlete.AthleteStatus = newSatatus.NewStatus!.Trim();

            await _context.SaveChangesAsync();
            await UpdateAthleteMongoSafe(athlete.loginAthlets, athlete.NameTeam, athlete.AthleteStatus);
            await ClearTeamCache(athlete.NameTeam);
        }

        public async Task LinkOrganizationTeam(OrganizationTeamDto entity)
        {
            if (string.IsNullOrWhiteSpace(entity.LoginOrganization))
                throw new CustomAppException("Не передано логін організації", 400, "LoginOrganization is required");

            if (string.IsNullOrWhiteSpace(entity.NameTeam))
                throw new CustomAppException("Не передано назву команди", 400, "NameTeam is required");

            entity.LoginOrganization = entity.LoginOrganization.Trim();
            entity.NameTeam = entity.NameTeam.Trim();

            await EnsureOrganizationExists(entity.LoginOrganization);
            await EnsureTeamExists(entity.NameTeam);

            var exists = await _context.OrganizationTeams
                .AsNoTracking()
                .AnyAsync(ot => ot.LoginOrganization == entity.LoginOrganization && ot.NameComand == entity.NameTeam);

            if (exists)
                return;

            var orgTeam = new OrganizationTeam
            {
                LoginOrganization = entity.LoginOrganization,
                NameComand = entity.NameTeam
            };

            _context.OrganizationTeams.Add(orgTeam);
            await _context.SaveChangesAsync();
            await ClearTeamCache(entity.NameTeam);
        }

        public async Task RemoveAthlet(NewSatatusAthlete newSatatus)
        {
            if (string.IsNullOrWhiteSpace(newSatatus.NameTeam))
                throw new CustomAppException("Не передано назву команди", 400, "NameTeam is required");

            if (string.IsNullOrWhiteSpace(newSatatus.LoginAthlete))
                throw new CustomAppException("Не передано логін спортсмена", 400, "LoginAthlete is required");

            var athlete = await GetAthlete(newSatatus.LoginAthlete, newSatatus.NameTeam);

            _context.teamAthletes.Remove(athlete);
            await _context.SaveChangesAsync();

            await UpdateAthleteMongoSafe(athlete.loginAthlets, string.Empty, string.Empty);
            await ClearTeamCache(athlete.NameTeam);
        }

        public async Task<TeamAthlete> GetAthlete(string loginAthlets, string nameTeam)
        {
            loginAthlets = loginAthlets?.Trim() ?? string.Empty;
            nameTeam = nameTeam?.Trim() ?? string.Empty;

            var athlete = await _context.teamAthletes
                .FirstOrDefaultAsync(a => a.loginAthlets == loginAthlets && a.NameTeam == nameTeam);

            if (athlete == null)
                throw new NotFoundException($"Спортсмен '{loginAthlets}' не знайдений у команді '{nameTeam}'");

            return athlete;
        }

        private static void NormalizeTeamDto(TeamModelDto team)
        {
            if (team == null)
                throw new CustomAppException("Команду не передано", 400, "Team is required");

            team.NameTeam = team.NameTeam?.Trim() ?? string.Empty;
            team.LoginTrainer = team.LoginTrainer?.Trim() ?? string.Empty;
            team.TypeSport = team.TypeSport?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(team.NameTeam))
                throw new CustomAppException("Назва команди обов'язкова", 400, "NameTeam is required");

            if (string.IsNullOrWhiteSpace(team.LoginTrainer))
                throw new CustomAppException("Логін тренера обов'язковий", 400, "LoginTrainer is required");

            if (string.IsNullOrWhiteSpace(team.TypeSport))
                throw new CustomAppException("Вид спорту обов'язковий", 400, "TypeSport is required");
        }

        private static List<TeamAthlete> BuildUniqueAthletes(string nameTeam, IEnumerable<TeamAthleteDto> athletes)
        {
            var result = new List<TeamAthlete>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var athlete in athletes ?? Enumerable.Empty<TeamAthleteDto>())
            {
                var login = athlete.LoginAthlets?.Trim();
                if (string.IsNullOrWhiteSpace(login) || !seen.Add(login))
                    continue;

                result.Add(new TeamAthlete
                {
                    NameTeam = nameTeam,
                    loginAthlets = login,
                    AthleteStatus = string.IsNullOrWhiteSpace(athlete.AthleteStatus)
                        ? "Active"
                        : athlete.AthleteStatus.Trim()
                });
            }

            return result;
        }

        private async Task EnsureTrainerExists(string loginTrainer)
        {
            var exists = await _context.Trainers
                .AsNoTracking()
                .AnyAsync(t => t.login == loginTrainer);

            if (!exists)
                throw new NotFoundException($"Тренера з логіном '{loginTrainer}' не знайдено. Спочатку створіть профіль тренера або введіть існуючий логін тренера.");
        }

        private async Task EnsureTeamExists(string nameTeam)
        {
            var exists = await _context.Teams
                .AsNoTracking()
                .AnyAsync(t => t.TeamName == nameTeam);

            if (!exists)
                throw new NotFoundException($"Команду '{nameTeam}' не знайдено");
        }

        private async Task EnsureOrganizationExists(string loginOrganization)
        {
            var exists = await _context.Organizations
                .AsNoTracking()
                .AnyAsync(o => o.login == loginOrganization);

            if (!exists)
                throw new NotFoundException($"Організацію з логіном '{loginOrganization}' не знайдено");
        }

        private async Task EnsureAthletesExist(IEnumerable<string?> athleteLogins)
        {
            var logins = athleteLogins
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (logins.Count == 0)
                return;

            var existing = await _context.Athletes
                .AsNoTracking()
                .Where(a => logins.Contains(a.login))
                .Select(a => a.login)
                .ToListAsync();

            var existingSet = existing.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var missing = logins.Where(login => !existingSet.Contains(login)).ToList();

            if (missing.Count > 0)
                throw new NotFoundException($"Не знайдено спортсменів: {string.Join(", ", missing)}");
        }

        private static void ValidateStatusRequest(NewSatatusAthlete newSatatus)
        {
            if (string.IsNullOrWhiteSpace(newSatatus.NameTeam))
                throw new CustomAppException("Не передано назву команди", 400, "NameTeam is required");

            if (string.IsNullOrWhiteSpace(newSatatus.LoginAthlete))
                throw new CustomAppException("Не передано логін спортсмена", 400, "LoginAthlete is required");

            if (string.IsNullOrWhiteSpace(newSatatus.NewStatus))
                throw new CustomAppException("Не передано новий статус спортсмена", 400, "NewStatus is required");

            newSatatus.NameTeam = newSatatus.NameTeam.Trim();
            newSatatus.LoginAthlete = newSatatus.LoginAthlete.Trim();
            newSatatus.NewStatus = newSatatus.NewStatus.Trim();
        }

        private async Task UpdateAthleteMongoSafe(string loginAthlete, string teamName, string status)
        {
            try
            {
                var filter = Builders<AthleteProfile>.Filter.Eq(a => a.login, loginAthlete);
                var update = Builders<AthleteProfile>.Update
                    .Set(a => a.Team, teamName)
                    .Set(a => a.Position, status)
                    .Set(a => a.dateLastUpdate, DateTime.UtcNow);

                await _playerProfile.UpdateOneAsync(filter, update);
            }
            catch
            {
                // Mongo profile sync should not break PostgreSQL team operation.
            }
        }

        private async Task ClearTeamCache(string nameTeam)
        {
            try
            {
                await _redisService.DeleteVerifacionCode(nameTeam + "Team");
                await _redisService.DeleteVerifacionCode(nameTeam + "Athlete");
            }
            catch
            {
                // Redis cache clear should not break main operation.
            }
        }
    }
}
