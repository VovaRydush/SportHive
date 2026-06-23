using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SportHive.Implementations.Background
{
    public static class MatchStatusNormalizer
    {
        public static async Task<int> NormalizeAsync(AppDbContext db, CancellationToken cancellationToken = default)
        {
            var total = 0;

            total += await NormalizeTableAsync(db, "IndividualMatch", cancellationToken);
            total += await NormalizeTableAsync(db, "TeamMatch", cancellationToken);
            total += await NormalizeTableAsync(db, "ExtremeMatch", cancellationToken);

            return total;
        }

        private static async Task<int> NormalizeTableAsync(AppDbContext db, string table, CancellationToken cancellationToken)
        {
            var finished = await db.Database.ExecuteSqlRawAsync($"""
                UPDATE "{table}"
                SET "StatusMatch" = 2
                WHERE "StatusMatch" <> 2
                  AND COALESCE("AddInformation", '') ~* '(finishedAt=|winner=[^;]+)'
            """, cancellationToken);

            var future = await db.Database.ExecuteSqlRawAsync($"""
                UPDATE "{table}"
                SET "StatusMatch" = 0
                WHERE "StatusMatch" = 1
                  AND "DataMatch" IS NOT NULL
                  AND ("DataMatch" + COALESCE("TimeMatch", interval '0')) > now()
                  AND COALESCE("AddInformation", '') !~* '(finishedAt=|winner=[^;]+)'
            """, cancellationToken);

            var live = await db.Database.ExecuteSqlRawAsync($"""
                UPDATE "{table}"
                SET "StatusMatch" = 1
                WHERE "StatusMatch" = 0
                  AND "DataMatch" IS NOT NULL
                  AND ("DataMatch" + COALESCE("TimeMatch", interval '0')) <= now()
                  AND COALESCE("AddInformation", '') !~* '(finishedAt=|winner=[^;]+)'
            """, cancellationToken);

            return finished + future + live;
        }
    }
}
