namespace SportHive.Services.Interfaces
{
    public interface ISystemSelectionService
    {
        Task RoundRobin();
        Task PlayOff();
        Task Mixsed();
        Task SwissSystem();
        Task QualificationByStandardsDorobitiPererobiti();
        Task Group();
        Task Final();
        Task Olympic();
        Task Knockout();
        Task DoubleElimination();
    }
}