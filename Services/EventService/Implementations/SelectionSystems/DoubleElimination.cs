using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class DoubleEliminationSystem : ICompetitionSystem
    {
        private readonly IMatchsGenerator _matchsGenerator;
        private readonly SaveMatchFactory _saveMatchFactory;
        private readonly AppDbContext _appDbContext;
        private readonly IMongoCollection<TeamIndivGrid> _sytemGrid;
        public DoubleEliminationSystem(IMatchsGenerator matchsGenerator, IMongoDbService mongoDbService,SaveMatchFactory saveMatchFactory,AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _saveMatchFactory = saveMatchFactory;
            _sytemGrid = mongoDbService.GetCollection<TeamIndivGrid>("TeamIndivGrid");
            _matchsGenerator = matchsGenerator;
        }
        public async Task GenerateFirstRoundAsync(Matchs matchs, long IdEvent)
        {
            await _matchsGenerator.GenerateInitialBracketAsync(matchs, IdEvent);
        }

        public async Task GenerateNextRoundAsync(Matchs matchs, long IdEvent)
        {
            var saveEntity = _saveMatchFactory.Create(matchs.typeSport);
            var allMatches = await _sytemGrid.Find(x => x.idEvent == IdEvent).ToListAsync();

            var topMatches = allMatches.OfType<TopGrig>().OrderBy(x => x.tour).ToList();
            var bottomMatches = allMatches.OfType<BottomGrid>().OrderBy(x => x.tour).ToList();

            int maxTopTour = topMatches.LastOrDefault()?.tour ?? 0;
            int maxBottomTour = bottomMatches.LastOrDefault()?.tour ?? 0;

            var lastTopTourMatches = topMatches.Where(m => m.tour == maxTopTour).ToList();
            var lastBottomTourMatches = bottomMatches.Where(m => m.tour == maxBottomTour).ToList();

            if (lastTopTourMatches.Any(m => !m.played) || lastBottomTourMatches.Any(m => !m.played))
                return;

            var topWinners = new List<string>();
            var topLosers = new List<string>();

            foreach (var match in lastTopTourMatches)
            {
                var winner = match.totalScoreEntity1 > match.totalScoreEntity2 ? match.NameFirstEntity : match.NameSecondEntity;
                var loser = match.totalScoreEntity1 > match.totalScoreEntity2 ? match.NameSecondEntity : match.NameFirstEntity;

                topWinners.Add(winner);
                topLosers.Add(loser);
            }

            var bottomWinners = new List<string>();
            var bottomLosers = new List<string>();

            foreach (var match in lastBottomTourMatches)
            {
                var winner = match.totalScoreEntity1 > match.totalScoreEntity2 ? match.NameFirstEntity : match.NameSecondEntity;
                var loser = match.totalScoreEntity1 > match.totalScoreEntity2 ? match.NameSecondEntity : match.NameFirstEntity;

                bottomWinners.Add(winner);
                bottomLosers.Add(loser);
            }

            if (topWinners.Count > 1)
            {
                var newTopMatches = new List<TopGrig>();
                for (int i = 0; i < topWinners.Count; i += 2)
                {
                    if (i + 1 >= topWinners.Count) break;

                    var newMatch = new TopGrig
                    {
                        idEvent = IdEvent,
                        idMatch = GenerateUniqueMatchId(),
                        tour = maxTopTour + 1,
                        NameFirstEntity = topWinners[i],
                        NameSecondEntity = topWinners[i + 1],
                        ParentMatch1 = lastTopTourMatches[i].idMatch.ToString(),
                        ParentMatch2 = lastTopTourMatches[i + 1].idMatch.ToString(),
                        played = false
                    };
                    newTopMatches.Add(newMatch);
                    await saveEntity.SaveMatch(_appDbContext, matchs, newMatch.NameFirstEntity, newMatch.NameSecondEntity, IdEvent);
                }

                await _sytemGrid.InsertManyAsync(newTopMatches);
            }

            var bottomCandidates = bottomWinners.Concat(topLosers).ToList();
            if (bottomCandidates.Count > 1)
            {
                var newBottomMatches = new List<BottomGrid>();
                for (int i = 0; i < bottomCandidates.Count; i += 2)
                {
                    if (i + 1 >= bottomCandidates.Count) break;

                    var newBottom = new BottomGrid
                    {
                        idEvent = IdEvent,
                        idMatch = GenerateUniqueMatchId(),
                        tour = maxBottomTour + 1,
                        NameFirstEntity = bottomCandidates[i],
                        NameSecondEntity = bottomCandidates[i + 1],
                        ParentMatch1 = "",
                        ParentMatch2 = "",
                        played = false
                    };
                    newBottomMatches.Add(newBottom);
                    await saveEntity.SaveMatch(_appDbContext, matchs, newBottom.NameFirstEntity, newBottom.NameSecondEntity, IdEvent);
                }

                await _sytemGrid.InsertManyAsync(newBottomMatches);
            }
        }
        private long GenerateUniqueMatchId()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

    }
}
