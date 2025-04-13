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

        public TeamOperateService(AppDbContext context){
            _context = context;
        }
        public async Task CreateTeamAsync(TeamModelDto Team)
        {
          var NameTeam = await _context.Teams.FirstOrDefaultAsync(nt => nt.TeamName == Team.NameTeam);
          if(NameTeam!=null) throw new NotFoundException("Команда з такою назвою вже існує");
        }
    }
}