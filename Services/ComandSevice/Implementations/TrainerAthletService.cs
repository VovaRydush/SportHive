using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Exceptions;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class TrainerAthletService : ITrainerAthletService
    {
        private readonly AppDbContext _context;

        public TrainerAthletService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TrainerProfileDto> GetTrainerProfileAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new NotFoundException("Trainer login is empty");

            var trainer = await _context.Trainers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.login == login);

            if (trainer == null)
                throw new NotFoundException("Trainer not found");

            var photo = await _context.UserPhotos
                .AsNoTracking()
                .Where(p => p.login == login)
                .Select(p => p.ProfilePhoto)
                .FirstOrDefaultAsync();

            var teams = await GetTrainerTeamsAsync(login);

            var organizations = new List<TrainerOrganizationDto>();

            try
            {
                organizations = await (
                    from ot in _context.OrganizationTrainers.AsNoTracking()
                    join org in _context.Organizations.AsNoTracking()
                        on ot.LoginOrganization equals org.login
                    where ot.LoginTraine == login
                    select new TrainerOrganizationDto
                    {
                        LoginOrganization = org.login,
                        NameOrganization = org.NameOrganization,
                        TypeOrganozation = org.TypeOrganozation,
                        Country = org.Country
                    }
                ).ToListAsync();
            }
            catch
            {
                organizations = new List<TrainerOrganizationDto>();
            }

            return new TrainerProfileDto
            {
                Login = trainer.login,
                FirsName = trainer.FirsName,
                LastName = trainer.LastName,
                DataBirth = trainer.DataBirth,
                Photo = photo,
                Teams = teams,
                Organizations = organizations,
                Stats = new TrainerStatsDto
                {
                    TeamsCount = teams.Count,
                    AthletesCount = teams.Sum(t => t.AthletesCount),
                    OrganizationsCount = organizations.Count,
                    TotalMatches = 0,
                    Wins = 0,
                    Draws = 0,
                    Losses = 0
                }
            };
        }

        public async Task<List<TrainerTeamDto>> GetTrainerTeamsAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                return new List<TrainerTeamDto>();

            var teams = await _context.Teams
                .AsNoTracking()
                .Where(t => t.LoginTrainer == login)
                .Select(t => new TrainerTeamDto
                {
                    NameTeam = t.TeamName,
                    TypeSport = t.TypeSport,
                    PhotoTeam = t.TeamPhoto,
                    AthletesCount = _context.teamAthletes.Count(a => a.NameTeam == t.TeamName)
                })
                .ToListAsync();

            return teams;
        }

        public Task AddAthletsToTrainer()
        {
            throw new NotImplementedException();
        }

        public Task ChangeStatusathlet()
        {
            throw new NotImplementedException();
        }

        public Task RemoveAthelte()
        {
            throw new NotImplementedException();
        }
    }
}
