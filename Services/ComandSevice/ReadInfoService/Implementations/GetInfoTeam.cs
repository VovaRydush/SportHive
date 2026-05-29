using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Exceptions;
using SportHive.Services.Interfaces;
using System.Text.Json;

namespace SportHive.Implementations
{
    class GetInfoTeam : IGetInfoTeam
    {
        private readonly AppDbContext _context;
        private readonly IRedisService _redisService;

        public GetInfoTeam(AppDbContext appDbContext, IRedisService redisService, IPhotoProcessing photoProcessing)
        {
            _context = appDbContext;
            _redisService = redisService;
        }

        public async Task<List<AthleteTeamDto>> GetAthetesAsync(string nameTeam)
        {
            if (string.IsNullOrWhiteSpace(nameTeam))
                throw new CustomAppException("Не передано назву команди", 400, "NameTeam is required");

            nameTeam = nameTeam.Trim();
            var cacheKey = nameTeam + "Athlete";

            var cached = await TryGetCache<List<AthleteTeamDto>>(cacheKey);
            if (cached != null)
                return cached;

            await EnsureTeamExists(nameTeam);

            var teamAthletes = await _context.teamAthletes
                .AsNoTracking()
                .Include(ta => ta.Athlete)
                .Where(ta => ta.NameTeam == nameTeam)
                .OrderBy(ta => ta.Athlete.LastName)
                .ThenBy(ta => ta.Athlete.FirsName)
                .Select(ta => new AthleteTeamDto
                {
                    FirsName = ta.Athlete.FirsName,
                    LastName = ta.Athlete.LastName,
                    login = ta.Athlete.login,
                    TypeSport = ta.Athlete.TypeSport,
                    AthleteStatus = ta.AthleteStatus,
                    Photo = _context.UserPhotos
                        .Where(p => p.login == ta.Athlete.login)
                        .Select(p => p.ProfilePhoto)
                        .FirstOrDefault()
                })
                .ToListAsync();

            await TrySetCache(cacheKey, teamAthletes, TimeSpan.FromMinutes(5));
            return teamAthletes;
        }

        public async Task<TeamInfoDto> GetTeamInfo(string nameTeam)
        {
            if (string.IsNullOrWhiteSpace(nameTeam))
                throw new CustomAppException("Не передано назву команди", 400, "NameTeam is required");

            nameTeam = nameTeam.Trim();
            var cacheKey = nameTeam + "Team";

            var cached = await TryGetCache<TeamInfoDto>(cacheKey);
            if (cached != null)
                return cached;

            var team = await LoadTeamQuery()
                .FirstOrDefaultAsync(t => t.TeamName == nameTeam);

            if (team == null)
                throw new NotFoundException($"Команду '{nameTeam}' не знайдено");

            var dto = MapTeam(team);
            await TrySetCache(cacheKey, dto, TimeSpan.FromMinutes(5));

            return dto;
        }

        public async Task<List<TeamInfoDto>> GetTeamsByOrganizationAsync(string loginOrganization)
        {
            if (string.IsNullOrWhiteSpace(loginOrganization))
                throw new CustomAppException("Не передано логін організації", 400, "LoginOrganization is required");

            loginOrganization = loginOrganization.Trim();

            var exists = await _context.Organizations
                .AsNoTracking()
                .AnyAsync(o => o.login == loginOrganization);

            if (!exists)
                throw new NotFoundException($"Організацію '{loginOrganization}' не знайдено");

            var teamNames = await _context.OrganizationTeams
                .AsNoTracking()
                .Where(ot => ot.LoginOrganization == loginOrganization)
                .Select(ot => ot.NameComand)
                .ToListAsync();

            if (teamNames.Count == 0)
                return new List<TeamInfoDto>();

            var teams = await LoadTeamQuery()
                .Where(t => teamNames.Contains(t.TeamName))
                .OrderBy(t => t.TeamName)
                .ToListAsync();

            return teams.Select(MapTeam).ToList();
        }

        public async Task<List<TeamInfoDto>> GetTeamsByTrainerAsync(string loginTrainer)
        {
            if (string.IsNullOrWhiteSpace(loginTrainer))
                throw new CustomAppException("Не передано логін тренера", 400, "LoginTrainer is required");

            loginTrainer = loginTrainer.Trim();

            var teams = await LoadTeamQuery()
                .Where(t => t.LoginTrainer == loginTrainer)
                .OrderBy(t => t.TeamName)
                .ToListAsync();

            return teams.Select(MapTeam).ToList();
        }

        private IQueryable<Team> LoadTeamQuery()
        {
            return _context.Teams
                .AsNoTracking()
                .Include(t => t.Trainer)
                .Include(t => t.TeamAthletes)
                    .ThenInclude(ta => ta.Athlete)
                .Include(t => t.OrganizationTeam!)
                    .ThenInclude(ot => ot.Organization);
        }

        private TeamInfoDto MapTeam(Team team)
        {
            return new TeamInfoDto
            {
                NameTeam = team.TeamName,
                TrainerFirstName = team.Trainer?.FirsName ?? string.Empty,
                TrainerLastName = team.Trainer?.LastName ?? string.Empty,
                TrainerLogin = team.LoginTrainer,
                TypeSport = team.TypeSport,
                TrainerPhotp = _context.UserPhotos
                    .Where(p => p.login == team.LoginTrainer)
                    .Select(p => p.ProfilePhoto)
                    .FirstOrDefault(),
                PhotoTeam = team.TeamPhoto,
                Athletes = (team.TeamAthletes ?? new List<TeamAthlete>())
                    .OrderBy(ta => ta.Athlete.LastName)
                    .ThenBy(ta => ta.Athlete.FirsName)
                    .Select(ta => new AthleteTeamDto
                    {
                        FirsName = ta.Athlete?.FirsName ?? string.Empty,
                        LastName = ta.Athlete?.LastName ?? string.Empty,
                        login = ta.loginAthlets,
                        TypeSport = ta.Athlete?.TypeSport,
                        AthleteStatus = ta.AthleteStatus,
                        Photo = _context.UserPhotos
                            .Where(p => p.login == ta.loginAthlets)
                            .Select(p => p.ProfilePhoto)
                            .FirstOrDefault()
                    })
                    .ToList(),
                Organizations = (team.OrganizationTeam ?? new List<OrganizationTeam>())
                    .Where(ot => ot.Organization != null)
                    .Select(ot => new OrganizationTeamInfoDto
                    {
                        LoginOrganization = ot.LoginOrganization,
                        NameOrganization = ot.Organization.NameOrganization,
                        Country = ot.Organization.Country,
                        TypeOrganozation = ot.Organization.TypeOrganozation
                    })
                    .ToList()
            };
        }

        private async Task EnsureTeamExists(string nameTeam)
        {
            var exists = await _context.Teams
                .AsNoTracking()
                .AnyAsync(t => t.TeamName == nameTeam);

            if (!exists)
                throw new NotFoundException($"Команду '{nameTeam}' не знайдено");
        }

        private async Task<T?> TryGetCache<T>(string key)
        {
            try
            {
                var value = await _redisService.GetEntity(key);
                return string.IsNullOrWhiteSpace(value)
                    ? default
                    : JsonSerializer.Deserialize<T>(value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return default;
            }
        }

        private async Task TrySetCache<T>(string key, T value, TimeSpan expiry)
        {
            try
            {
                await _redisService.SetEntity(key, JsonSerializer.Serialize(value), expiry);
            }
            catch
            {
                // Redis is optional for read model.
            }
        }
    }
}
