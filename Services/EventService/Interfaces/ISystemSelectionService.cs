using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface ISystemSelectionService
    {
        Task RoundRobin(MatchsAbstractionDto match);
        Task PlayOff(MatchsAbstractionDto match);
        Task Mixsed(MatchsAbstractionDto match);
        Task SwissSystem(MatchsAbstractionDto match);
        Task QualificationByStandardsDorobitiPererobiti(MatchsAbstractionDto match);
        Task Group(MatchsAbstractionDto match);
        Task Olympic(MatchsAbstractionDto match);
        Task Knockout(MatchsAbstractionDto match);
        Task DoubleElimination(MatchsAbstractionDto match);

        Task CreateMatchWithSSystem(MatchsAbstractionDto teamComposition);
    }
}