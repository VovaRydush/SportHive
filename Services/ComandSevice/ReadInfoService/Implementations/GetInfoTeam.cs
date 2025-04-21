using SportHive.Services.Interfaces;
using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Exceptions;
using System.Text.Json;

namespace SportHive.Implementations
{
    class GetInfoTeam : IGetInfoTeam
    {
        private readonly AppDbContext _context;
        private readonly IRedisService _redisService;
        private readonly IPhotoProcessing _photoProcessing;

        public GetInfoTeam(AppDbContext appDbContext, IRedisService redisService, IPhotoProcessing photoProcessing)
        {
            _context = appDbContext;
            _redisService = redisService;
            _photoProcessing = photoProcessing;
        }

        public async Task<List<AthleteTeamDto>> GetAthetesAsync(string NameTeam)
        {
            var teamAthletesRedis = await _redisService.GetEntity(NameTeam + "Athlete");
            if (teamAthletesRedis != null)
            {
                string json = string.Join(",", teamAthletesRedis);
                List<AthleteTeamDto> Athletes = JsonSerializer.Deserialize<List<AthleteTeamDto>>(json);
                return Athletes;
            }
            else
            {
                var teamAthletes = await _context.teamAthletes
                    .AsNoTracking()
                    .Include(ta => ta.Athlete)
                    .Where(ta => ta.NameTeam == NameTeam)
                    .Select(athlete => new AthleteTeamDto
                    {
                        FirsName = athlete.Athlete.FirsName,
                        LastName = athlete.Athlete.LastName,
                        login = athlete.Athlete.login
                    })
                    .ToListAsync();
                var athlete = JsonSerializer.Serialize(teamAthletes);
                await _redisService.SetEntity(NameTeam + "Athlete", athlete, TimeSpan.FromMinutes(20));
                return teamAthletes;
            }

        }

        public async Task<TeamInfoDto> GetTeamInfo(string NameTeam)
        {
            var teamRedis = await _redisService.GetEntity(NameTeam + "Team");
            if (teamRedis != null)
            {
                var Team = JsonSerializer.Deserialize<TeamInfoDto>(teamRedis);
                return Team;
            }
            else
            {
                var photoPathTeam = await _context.UserPhotos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(photo=> photo.login== NameTeam);
                //var path = await _photoProcessing.GetPhotoBase64Async(photoPathTeam.ProfilePhoto);
                var team = await _context.Teams
                .AsNoTracking()
                .Include(t => t.Trainer)
                .Where(ta => ta.TeamName == NameTeam)
                .Select(teams => new TeamInfoDto
                {
                    NameTeam = teams.TeamName,
                    TrainerFirstName = teams.Trainer.FirsName,
                    TrainerLastName = teams.Trainer.LastName,
                    TrainerLogin = teams.Trainer.login,
                    TypeSport = teams.TypeSport,
                    TrainerPhotp = "path",
                    PhotoTeam = "path"
                }).FirstOrDefaultAsync();

                var athlete = JsonSerializer.Serialize(team);
                await _redisService.SetEntity(NameTeam + "Team", athlete, TimeSpan.FromMinutes(20));
                return team;
            }

        }
    }
}