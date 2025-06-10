using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SingleElimination : ICompetitionSystem
    {
        private readonly IMatchsGenerator _matchsGenerator;
        private readonly SaveMatchFactory _saveMatchFactory;
        private readonly AppDbContext _appDbContext;
        private readonly IMongoCollection<TeamIndivGrid> _sytemGrid;
        public SingleElimination(AppDbContext appDbContext,IMatchsGenerator matchsGenerator, IMongoDbService mongoDbService,SaveMatchFactory saveMatchFactory)
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
            var allMatches = await _sytemGrid.Find(x => x.idEvent == IdEvent && x is SingleEliminationGrid)
                                      .ToListAsync();

            var lastTour = allMatches
                .Where(x => x is SingleEliminationGrid)
                .Max(x => x.tour);

            var lastTourMatches = allMatches
                .Where(x => x.tour == lastTour)
                .Cast<SingleEliminationGrid>()
                .ToList();

            if (lastTourMatches.Any(x => !x.played))
                return;

            var winners = lastTourMatches
                .Select(m => m.totalScoreEntity1 > m.totalScoreEntity2 ? m.NameFirstEntity : m.NameSecondEntity)
                .ToList();

            if (winners.Count <= 1)
                return;

            var newMatches = new List<SingleEliminationGrid>();
            int nextTour = lastTour + 1;

            for (int i = 0; i < winners.Count; i += 2)
            {
                if (i + 1 >= winners.Count) break;

                var parent1 = lastTourMatches[i].idMatch.ToString();
                var parent2 = lastTourMatches[i + 1].idMatch.ToString();

                var match = new SingleEliminationGrid
                {
                    idEvent = IdEvent,
                    idMatch = GenerateUniqueMatchId(),
                    tour = nextTour,
                    NameFirstEntity = winners[i],
                    NameSecondEntity = winners[i + 1],
                    ParentMatch1 = parent1,
                    ParentMatch2 = parent2,
                    NameWinner = string.Empty,
                    played = false
                };

                newMatches.Add(match);
                
                await saveEntity.SaveMatch(_appDbContext, matchs, match.NameFirstEntity, match.NameSecondEntity, IdEvent);
            }
            if (newMatches.Count > 0)
                await _sytemGrid.InsertManyAsync(newMatches);
        }
        private long GenerateUniqueMatchId()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

    }
}