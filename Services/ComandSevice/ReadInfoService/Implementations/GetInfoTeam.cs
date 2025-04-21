using SportHive.Services.Interfaces;
using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Exceptions;

namespace SportHive.Implementations
{
    class GetInfoTeam : IGetInfoTeam
    {
        
        public Task<List<AthleteTeamDto>> GetAthetesAsync(string NameTeam)
        {
            throw new NotImplementedException();
        }

        public Task GetTeam()
        {
            throw new NotImplementedException();
        }
    }
}