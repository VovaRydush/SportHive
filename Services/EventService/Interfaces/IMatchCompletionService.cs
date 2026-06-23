namespace SportHive.Services.Interfaces
{
    public interface IMatchCompletionService
    {
        Task<int> NormalizeAllAsync(CancellationToken cancellationToken = default);
        Task<CompleteMatchResult> CompleteMatchAsync(CompleteMatchRequest request, CancellationToken cancellationToken = default);
    }

    public sealed class CompleteMatchRequest
    {
        public string MatchType { get; set; } = "";
        public long MatchId { get; set; }
        public string? Score { get; set; }
        public string? Winner { get; set; }
        public string? Notes { get; set; }
    }

    public sealed class CompleteMatchResult
    {
        public bool Success { get; set; }
        public int ChangedRows { get; set; }
        public string Message { get; set; } = "";
    }
}
