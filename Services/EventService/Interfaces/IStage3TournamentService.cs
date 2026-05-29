using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface IStage3TournamentService
    {
        Task<Stage3EventWorkspaceDto> CreateEventAsync(Stage3CreateEventDto dto);
        Task<Stage3EventWorkspaceDto> GenerateMatchesAsync(Stage3GenerateMatchesDto dto);
        Task<Stage3EventWorkspaceDto> GetWorkspaceAsync(string eventName);
        Task<Stage3MatchDto> GetMatchAsync(string matchType, long idMatch);
        Task<Stage3EventWorkspaceDto> SetResultAsync(Stage3SetMatchResultDto dto);
        Task<Stage3MatchDto> ChangeStatusAsync(Stage3ChangeMatchStatusDto dto);
    }
}
