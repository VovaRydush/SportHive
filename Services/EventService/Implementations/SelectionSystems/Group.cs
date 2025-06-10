using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.VisualBasic;
using SportHive.Models;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class GroupSystem : ICompetitionSystem
    {
        private readonly SaveMatchFactory _saveMatchFactory;
        private readonly IInitalSystemGrid _initalSystemGrid;
        private readonly AppDbContext _appDbContext;
        public GroupSystem(SaveMatchFactory saveMatchFactory, AppDbContext appDbContext, IInitalSystemGrid initalSystemGrid)
        {
            _initalSystemGrid = initalSystemGrid;
            _appDbContext = appDbContext;
            _saveMatchFactory = saveMatchFactory;
        }
        public async Task GenerateFirstRoundAsync(Matchs matchs, long IdEvent)
        {
            var saveEntity = _saveMatchFactory.Create(matchs.typeSport);
            GroupDivision isDivisionGroups = await DivisionGroups(matchs.Entitys);
            int totalTeams = matchs.Entitys.Count;
            int skipped = isDivisionGroups.countSkip;
            int mainTeamsCount = totalTeams - skipped;

            if (isDivisionGroups.countSkip != 0)
            {
                var skippedTeams = matchs.Entitys.TakeLast(isDivisionGroups.countSkip).ToList();

                for (int i = 0; i < skippedTeams.Count - 1; i++)
                {
                    for (int j = i + 1; j < skippedTeams.Count; j++)
                    {
                        matchs.Group = 1;
                        await saveEntity.SaveMatch(_appDbContext, matchs, skippedTeams[i], skippedTeams[j], IdEvent);
                        await _initalSystemGrid.InitalSystemGrids(matchs.IdEvent, skippedTeams[i], skippedTeams[j], matchs);
                    }
                }
            }
            var mainTeams = matchs.Entitys.Take(mainTeamsCount).ToList();
            for (int i = 0; i < mainTeams.Count - 1; i++)
            {
                for (int j = i + 1; j < mainTeams.Count; j++)
                {
                    matchs.Group = i + 1;
                    await saveEntity.SaveMatch(_appDbContext, matchs, mainTeams[i], mainTeams[j], IdEvent);
                    await _initalSystemGrid.InitalSystemGrids(matchs.IdEvent, mainTeams[i], mainTeams[j], matchs);
                }
            }
            await _appDbContext.SaveChangesAsync();
        }

        public Task GenerateNextRoundAsync(Matchs matchs, long IdEvent)
        {
            throw new NotImplementedException();
        }
        public Task<GroupDivision> DivisionGroups(List<string> entitys)
        {
            var result = new GroupDivision();
            int n = entitys.Count;
            int absoluteMinGroupSize = 2;
            int preferredMin = 8;
            int preferredMax = 12;

            int bestGroupSize = -1;
            int bestAdjustedN = -1;
            int bestFullGroups = 0;
            int minRemainder = int.MaxValue;

            int maxGroupSize = Math.Min(preferredMax, n);

            for (int groupSize = absoluteMinGroupSize; groupSize <= maxGroupSize; groupSize++)
            {
                for (int reduce = 0; reduce <= Math.Min(3, n); reduce++)
                {
                    int adjustedN = n - reduce;
                    if (adjustedN < groupSize) continue;

                    int fullGroups = adjustedN / groupSize;
                    int remainder = adjustedN % groupSize;

                    if (remainder == 0 && fullGroups > 0)
                    {
                        bestGroupSize = groupSize;
                        bestAdjustedN = adjustedN;
                        bestFullGroups = fullGroups;
                        minRemainder = 0;
                        break;
                    }
                    else if (remainder < minRemainder || (remainder == minRemainder && fullGroups > bestFullGroups))
                    {
                        bestGroupSize = groupSize;
                        bestAdjustedN = adjustedN;
                        bestFullGroups = fullGroups;
                        minRemainder = remainder;
                    }
                }
            }

            if (bestGroupSize == -1)
            {
                result.countGroups = 1;
                result.countEntitys = n;
                result.countSkip = 0;
                return Task.FromResult(result);
            }

            result.countGroups = bestFullGroups;
            result.countEntitys = bestGroupSize;
            result.countSkip = n - bestAdjustedN;

            return Task.FromResult(result);
        }
    }
}
