using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface ISportRulesService
    {
        List<SportRuleDto> GetRules();
        SportRuleDto GetRule(string sport);
        Task<SportMatchStateDto> GetMatchStateAsync(string matchType, long matchId, string? login, string? role);
        Task<SportMatchStateDto> AddLiveEventAsync(SportLiveEventSubmitDto dto);
        Task<SportMatchStateDto> SubmitFinalResultAsync(SportFinalResultSubmitDto dto);
        SportScoreValidationResultDto ValidateFinalScore(string sport, string score, string firstParticipant, string secondParticipant, string? winner);
    }
}
