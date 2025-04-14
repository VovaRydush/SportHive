using SportHive.Services.Interfaces;
using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Exceptions;
namespace SportHive.Implementations
{
    class TeamOperateService : ITeamOperateService
    {
        private readonly AppDbContext _context;

        public TeamOperateService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAthletes(List<TeamAthleteDto> athleteDto)
        {
            var entities = athleteDto.Select(d => new TeamAthlete
            {
                NameTeam = d.NameTeam,
                loginAthlets = d.loginAthlets,
                AthleteStatus = d.AthleteStatus
            }).ToList();

            _context.teamAthletes.AddRange(entities);

            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusAthlete(NewSatatusAthlete newSatatus)
        {
           var athlet = await GetAthlete(newSatatus.LoginAthlete,newSatatus.NameTeam);
           athlet.AthleteStatus= newSatatus.NewStatus;
           await  _context.SaveChangesAsync();
        }

        public async Task CreateTeamAsync(TeamModelDto team)
        {
            var NameTeam = await _context.Teams.AsNoTracking().FirstOrDefaultAsync(nt => nt.TeamName == team.NameTeam);
            if (NameTeam != null) throw new NotFoundException("Команда з такою назвою вже існує");

            var Command = new Team
            {
                TeamName = team.NameTeam,
                LoginTrainer = team.LoginTrainer,
                TypeSport = team.TypeSport
            };
            _context.Teams.Add(Command);

           
            var entities = team.athlets.Select(d => new TeamAthlete
            {
                NameTeam = d.NameTeam,
                loginAthlets = d.loginAthlets,
                AthleteStatus = d.AthleteStatus
            }).ToList();

            _context.teamAthletes.AddRange(entities);

            await _context.SaveChangesAsync();

        }

        public async Task<TeamAthlete> GetAthlete(string loginAthlets, string NameTeam)
        {
           return await _context.teamAthletes.FirstAsync(a => a.loginAthlets == loginAthlets && a.NameTeam == NameTeam);
        }

        public async Task LinkOrganizationTeam(OrganizationTeamDto entity)
        {
            var OrgTeam = new OrganizationTeam{
                LoginOrganization = entity.LoginOrganization,
                NameComand = entity.NameTeam
            };
            _context.OrganizationTeams.Add(OrgTeam);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAthlet(NewSatatusAthlete newSatatus)
        {
           var athlet = await GetAthlete(newSatatus.LoginAthlete,newSatatus.NameTeam);
           _context.teamAthletes.Remove(athlet);
           await _context.SaveChangesAsync();
        }
    }
}