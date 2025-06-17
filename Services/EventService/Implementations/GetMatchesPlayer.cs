using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SportHive.Exceptions;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class GetMatchesPlayer : IGetMatchesPlayer
    {
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        private readonly IMongoCollection<MatchEvents> _matchEvents;
        private readonly AppDbContext _appDbContext;
        private readonly IPhotoProcessing _photoProcessing;

        public GetMatchesPlayer(IMongoDbService mongoDbService, AppDbContext appDbContext, IPhotoProcessing photoProcessing)
        {
            _photoProcessing = photoProcessing;
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
            _matchEvents = mongoDbService.GetCollection<MatchEvents>("MatchEvents");
            _appDbContext = appDbContext;
        }

        public async Task<TeamIndivMatch> GetIndividualMatches(string loginUser)
        {
            var filter = Builders<AthleteProfile>.Filter.Eq(x => x.login, loginUser);
            var athlete = await _playerProfile.Find(filter).FirstOrDefaultAsync();
            if (athlete == null)
                throw new NotFoundException("Спортсмена не знайдено");
            var filterMatches = Builders<MatchEvents>.Filter.AnyEq(m => m.composition, athlete.FullName);
            var matches = await _matchEvents.Find(filterMatches).ToListAsync();
            var result = new TeamIndivMatch
            {
                TypeSport = "Індивідуальний",
                Matches = new List<TeamIndivMatchRes>()
            };
            foreach (var match in matches)
            {
                string opponent = match.composition.FirstOrDefault(name => name != athlete.FullName) ?? "Unknown";
                string status = "Draw";

                if (match.NameWinner == athlete.FullName)
                    status = "Win";
                else if (match.NameLosser == athlete.FullName)
                    status = "Loss";
                else if (match.Draws?.Contains(athlete.FullName) == true)
                    status = "Draw";
                var photoTeam = await GetTeamPhoto(athlete.login);
                var photoOpponent = await GetTeamPhoto(opponent);

                result.Matches.Add(new TeamIndivMatchRes
                {
                    firstTeamScore = match.firstTeamScore,
                    secondTeamScore = match.secondTeamScore,
                    NameEntity1 = athlete.FullName,
                    photoFirstEntity = photoTeam,
                    NameEntity2 = opponent,
                    photoSecondEntity = photoOpponent,
                    statusMatch = status
                });
            }
            return result;  
        }

        public async Task<TeamIndivMatch> GetTeamMatches(string loginUser)
        {
            var filter = Builders<AthleteProfile>.Filter.Eq(x => x.login, loginUser);
            var athlete = await _playerProfile.Find(filter).FirstOrDefaultAsync();

            if (athlete == null || string.IsNullOrEmpty(athlete.Team))
                throw new NotFoundException("Атлет або команда не знайдені");

            string teamName = athlete.Team;

            var filterMatches = Builders<MatchEvents>.Filter.AnyEq(m => m.composition, teamName);
            var matches = await _matchEvents.Find(filterMatches).ToListAsync();

            var result = new TeamIndivMatch
            {
                TypeSport = "Командний",
                Matches = new List<TeamIndivMatchRes>()
            };
            foreach (var match in matches)
            {
                string opponent = match.composition.FirstOrDefault(name => name != teamName) ?? "Unknown";
                string status = "Draw";

                if (match.NameWinner == teamName)
                    status = "Win";
                else if (match.NameLosser == teamName)
                    status = "Loss";
                else if (match.Draws?.Contains(teamName) == true)
                    status = "Draw";
                var photoTeam = await GetTeamPhoto(teamName);
                var photoOpponent = await GetTeamPhoto(opponent);

                result.Matches.Add(new TeamIndivMatchRes
                {
                    firstTeamScore = match.firstTeamScore,
                    secondTeamScore = match.secondTeamScore,
                    NameEntity1 = teamName,
                    photoFirstEntity = photoTeam,
                    NameEntity2 = opponent,
                    photoSecondEntity = photoOpponent,
                    statusMatch = status
                });
            }
            return result;
        }
        public async Task<string> GetTeamPhoto(string entity)
        {
            var path = _appDbContext.Teams.AsNoTracking().Where(x => x.TeamName == entity).Select(p => p.TeamPhoto).FirstOrDefault();
            return await _photoProcessing.GetPhotoBase64Async(path ?? "");
        }
    }
}