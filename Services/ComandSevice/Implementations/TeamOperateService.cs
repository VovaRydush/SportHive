using SportHive.Services.Interfaces;
using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Exceptions;
using System.Text.Json;
namespace SportHive.Implementations
{
    class TeamOperateService : ITeamOperateService
    {
        private readonly AppDbContext _context;
        private readonly IPhotoProcessing _photoProcessing;
        private readonly ISaveDataDb _saveDataDb;
        public TeamOperateService(AppDbContext context,IPhotoProcessing photoProcessing,ISaveDataDb saveDataDb )
        {
            _saveDataDb = saveDataDb;
            _photoProcessing = photoProcessing;
            _context = context;
        }

        public async Task AddAthletes(List<TeamAthleteDto> athleteDto)
        {
            var entities = athleteDto.Select(d => new TeamAthlete
            {
                NameTeam = d.NameTeam,
                loginAthlets = d.LoginAthlets,
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

            string photoPath = null;
            if (team.Photo != null)
            {
                photoPath = await _photoProcessing.SavePhotoAsync(team.Photo);
            }

            var jsonObJuserPhoto = JsonSerializer.Serialize(new UserPhoto
            {
                login = team.NameTeam,
                ProfilePhoto = photoPath
            });
           _ = _saveDataDb.SaveDataToDb(jsonObJuserPhoto, "user-photo");

            var Command = new Team
            {
                TeamName = team.NameTeam,
                LoginTrainer = team.LoginTrainer,
                TypeSport = team.TypeSport,
                TeamPhoto = photoPath
            };
            _context.Teams.Add(Command);

           
            var entities = team.Athlets.Select(d => new TeamAthlete
            {
                NameTeam = d.NameTeam,
                loginAthlets = d.LoginAthlets,
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