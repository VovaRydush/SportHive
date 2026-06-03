using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface IEventCatalogService
    {
        Task<List<EventCatalogItemDto>> GetCatalogAsync(EventCatalogQueryDto query);
        Task<EventCatalogItemDto> GetEventAsync(long idEvent, string? login, string? role);
        Task<EventMatchCatalogDto> SubmitResultAsync(MatchResultSubmitDto dto);
        Task<EventCatalogItemDto> GenerateNextRoundAsync(long idEvent, string? login, string? role);
        Task<EventCatalogItemDto> RebuildEventAsync(long idEvent, string? login, string? role);
    }
}
