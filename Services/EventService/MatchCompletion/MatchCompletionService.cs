using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Services.Interfaces;
using System.Text.RegularExpressions;

namespace SportHive.Implementations.MatchCompletion
{
    public sealed class MatchCompletionService : IMatchCompletionService
    {
        private readonly AppDbContext _db;

        public MatchCompletionService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> NormalizeAllAsync(CancellationToken cancellationToken = default)
        {
            var total = 0;

            total += await NormalizeTableAsync("IndividualMatch", cancellationToken);
            total += await NormalizeTableAsync("TeamMatch", cancellationToken);
            total += await NormalizeTableAsync("ExtremeMatch", cancellationToken);

            return total;
        }

        public async Task<CompleteMatchResult> CompleteMatchAsync(CompleteMatchRequest request, CancellationToken cancellationToken = default)
        {
            var table = ResolveTable(request.MatchType);
            var idColumn = ResolveIdColumn(request.MatchType);

            if (table is null || idColumn is null)
            {
                return new CompleteMatchResult
                {
                    Success = false,
                    Message = "Невідомий тип матчу"
                };
            }

            var score = CleanValue(request.Score);
            var winner = CleanValue(request.Winner);
            var notes = CleanValue(request.Notes);

            if (string.IsNullOrWhiteSpace(score) && string.IsNullOrWhiteSpace(winner))
            {
                return new CompleteMatchResult
                {
                    Success = false,
                    Message = "Для завершення матчу потрібен рахунок або переможець"
                };
            }

            var finalInfo = BuildFinalInformation(score, winner, notes);

            var changed = await _db.Database.ExecuteSqlRawAsync($"""
                UPDATE "{table}"
                SET "StatusMatch" = 2,
                    "AddInformation" = trim(both ';' from @p0 || ';' || COALESCE("AddInformation", ''))
                WHERE "{idColumn}" = @p1
            """, [finalInfo, request.MatchId], cancellationToken);

            return new CompleteMatchResult
            {
                Success = changed > 0,
                ChangedRows = changed,
                Message = changed > 0 ? "Матч завершено" : "Матч не знайдено"
            };
        }

        private async Task<int> NormalizeTableAsync(string table, CancellationToken cancellationToken)
        {
            var matchDateTime = MatchDateTimeSql();

            var changed = 0;

            changed += await _db.Database.ExecuteSqlRawAsync($"""
                UPDATE "{table}"
                SET "StatusMatch" = 2,
                    "AddInformation" = CASE
                        WHEN COALESCE("AddInformation", '') !~* 'finishedAt\s*='
                        THEN trim(both ';' from COALESCE("AddInformation", '') || ';finishedAt=' || to_char(now() AT TIME ZONE 'UTC', 'YYYY-MM-DD"T"HH24:MI:SS.MS"Z"'))
                        ELSE "AddInformation"
                    END
                WHERE "StatusMatch" <> 2
                  AND COALESCE("AddInformation", '') ~* '(finishedAt\s*=|winner\s*=|status\s*=\s*(finished|completed|2)|matchFinished\s*=\s*(true|1))'
            """, cancellationToken);

            changed += await _db.Database.ExecuteSqlRawAsync($"""
                UPDATE "{table}"
                SET "StatusMatch" = 0
                WHERE "StatusMatch" <> 2
                  AND "DataMatch" IS NOT NULL
                  AND {matchDateTime} > now()
                  AND COALESCE("AddInformation", '') !~* '(finishedAt\s*=|winner\s*=|status\s*=\s*(finished|completed|2)|matchFinished\s*=\s*(true|1))'
            """, cancellationToken);

            changed += await _db.Database.ExecuteSqlRawAsync($"""
                UPDATE "{table}"
                SET "StatusMatch" = 1
                WHERE "StatusMatch" <> 2
                  AND "DataMatch" IS NOT NULL
                  AND {matchDateTime} <= now()
                  AND COALESCE("AddInformation", '') !~* '(finishedAt\s*=|winner\s*=|status\s*=\s*(finished|completed|2)|matchFinished\s*=\s*(true|1))'
            """, cancellationToken);

            return changed;
        }

        private static string MatchDateTimeSql()
        {
            return """
                (
                    "DataMatch"::date
                    + COALESCE(
                        NULLIF("TimeMatch"::text, '')::time,
                        time '00:00'
                    )
                )
            """;
        }

        private static string? ResolveTable(string matchType)
        {
            var type = NormalizeType(matchType);

            return type switch
            {
                "individual" => "IndividualMatch",
                "team" => "TeamMatch",
                "extreme" => "ExtremeMatch",
                _ => null
            };
        }

        private static string? ResolveIdColumn(string matchType)
        {
            var type = NormalizeType(matchType);

            return type switch
            {
                "individual" => "IdIndividualMatch",
                "team" => "IdTeamMatch",
                "extreme" => "IdExtremeMatches",
                _ => null
            };
        }

        private static string NormalizeType(string matchType)
        {
            var value = (matchType ?? "").Trim().ToLowerInvariant();

            if (value.Contains("individual")) return "individual";
            if (value.Contains("team")) return "team";
            if (value.Contains("extreme")) return "extreme";

            return value;
        }

        private static string BuildFinalInformation(string score, string winner, string notes)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(score))
                parts.Add($"score={score}");

            if (!string.IsNullOrWhiteSpace(winner))
                parts.Add($"winner={winner}");

            parts.Add($"finishedAt={DateTime.UtcNow:O}");
            parts.Add("status=Finished");
            parts.Add("matchFinished=true");

            if (!string.IsNullOrWhiteSpace(notes))
                parts.Add($"notes={notes}");

            return string.Join(";", parts);
        }

        private static string CleanValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            var cleaned = value.Trim();
            cleaned = cleaned.Replace(";", " ");
            cleaned = Regex.Replace(cleaned, @"\s+", " ");

            return cleaned;
        }
    }
}
