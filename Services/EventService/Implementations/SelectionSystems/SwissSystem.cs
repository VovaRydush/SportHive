using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using MongoDB.Driver;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class SwissSystem : ICompetitionSystem
    {
        private readonly IMatchsGenerator _matchsGenerator;
        private readonly AppDbContext _appDbContext;
        private readonly SaveMatchFactory _saveMatchFactory;
        private readonly IMongoCollection<TeamIndivGrid> _sytemGrid;
        private readonly IMongoCollection<SwissSystemPlayed> _playedCollection;
        private readonly IMongoCollection<AthleteProfile> _playerProfile;
        public SwissSystem(IMatchsGenerator matchsGenerator, IMongoDbService mongoDbService, SaveMatchFactory saveMatchFactory,AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _playerProfile = mongoDbService.GetCollection<AthleteProfile>("PlayerProfile");
            _saveMatchFactory = saveMatchFactory;
            _sytemGrid = mongoDbService.GetCollection<TeamIndivGrid>("TeamIndivGrid");
            _playedCollection = mongoDbService.GetCollection<SwissSystemPlayed>("SwissSystemPlayed");
            _matchsGenerator = matchsGenerator;
        }
        public async Task GenerateFirstRoundAsync(Matchs matchs, long IdEvent)
        {
            if (matchs.Rating == false) await GenerateFirstRoundRandomAsync(matchs, IdEvent);
            else await GenerateFirstRoundByRatingAsync(matchs, IdEvent);
        }

        public async Task GenerateNextRoundAsync(Matchs matchs, long IdEvent)
        {
            var allMatches = await _sytemGrid.Find(x => x.idEvent == IdEvent).ToListAsync();
            var lastTour = allMatches.Any() ? allMatches.Max(x => x.tour) : 0;
            var saveEntity = _saveMatchFactory.Create(matchs.typeSport);
            // Підрахунок очок
            var scores = new Dictionary<string, int>();
            foreach (var match in allMatches.Where(x => x.played))
            {
                if (!scores.ContainsKey(match.NameFirstEntity))
                    scores[match.NameFirstEntity] = 0;
                if (!scores.ContainsKey(match.NameSecondEntity))
                    scores[match.NameSecondEntity] = 0;

                if (match.totalScoreEntity1 > match.totalScoreEntity2)
                    scores[match.NameFirstEntity] += 3;
                else if (match.totalScoreEntity2 > match.totalScoreEntity1)
                    scores[match.NameSecondEntity] += 3;
                else
                {
                    scores[match.NameFirstEntity] += 1;
                    scores[match.NameSecondEntity] += 1;
                }
            }

            // Отримання списку зіграних пар
            var playedMatches = await _playedCollection.Find(x => x.IdEvent == IdEvent).ToListAsync();
            var playedSet = new HashSet<string>(playedMatches.Select(x => $"{x.NameFirstEntity}_{x.NameSecondEntity}")
                                                             .Concat(playedMatches.Select(x => $"{x.NameSecondEntity}_{x.NameFirstEntity}")));

            var allEntities = scores.Keys.ToList();

            // Врахування рейтингу (опціонально)
            Dictionary<string, int> ratings = matchs.Rating ? await GetRatingsForEntities(allEntities) : new();

            var sortedEntities = matchs.Rating
                ? scores.OrderByDescending(x => x.Value).ThenByDescending(x => ratings.GetValueOrDefault(x.Key, 1000)).Select(x => x.Key).ToList()
                : scores.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();

            var newMatches = new List<TeamIndivGrid>();
            var used = new HashSet<string>();
            var nextTour = lastTour + 1;

            for (int i = 0; i < sortedEntities.Count; i++)
            {
                if (used.Contains(sortedEntities[i])) continue;

                for (int j = i + 1; j < sortedEntities.Count; j++)
                {
                    if (used.Contains(sortedEntities[j])) continue;

                    var key = $"{sortedEntities[i]}_{sortedEntities[j]}";
                    if (!playedSet.Contains(key))
                    {
                        var newMatch = new TeamIndivGrid
                        {
                            idEvent = IdEvent,
                            idMatch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            tour = nextTour,
                            NameFirstEntity = sortedEntities[i],
                            NameSecondEntity = sortedEntities[j],
                            played = false,
                            falloutLoser = false
                        };

                        newMatches.Add(newMatch);

                        // Save to SwissSystemPlayed to avoid repetition
                        var played = new SwissSystemPlayed
                        {
                            IdEvent = IdEvent,
                            NameFirstEntity = sortedEntities[i],
                            NameSecondEntity = sortedEntities[j]
                        };
                        await _playedCollection.InsertOneAsync(played);

                        await saveEntity.SaveMatch(_appDbContext, matchs, sortedEntities[i], sortedEntities[j], IdEvent);

                        used.Add(sortedEntities[i]);
                        used.Add(sortedEntities[j]);
                        break;
                    }
                }
            }

            if (newMatches.Any())
            {
                await _sytemGrid.InsertManyAsync(newMatches);
            }
        }

        private async Task<Dictionary<string, int>> GetRatingsForEntities(List<string> names)
        {
            var result = new Dictionary<string, int>();
            foreach (var name in names)
            {
                result[name] = await GetRating(name);
            }
            return result;
        }

        private async Task<int> GetRating(string name)
        {
            var filter = Builders<AthleteProfile>.Filter.Eq(x => x.FullName, name);
            var profile = await _playerProfile.Find(filter).FirstOrDefaultAsync();

            if (profile?.SportStats is CheckersChessStats stats)
                return stats.EloRating;

            return 1000; 
        }

        public async Task GenerateFirstRoundRandomAsync(Matchs matchs, long IdEvent)
        {
            await _matchsGenerator.GenerateInitialBracketAsync(matchs, IdEvent);
        }
        public async Task GenerateFirstRoundByRatingAsync(Matchs matchs, long IdEvent)
        {
            await Task.CompletedTask;
        }
    }
}
