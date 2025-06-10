using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface IEnterDataMatches
    {
        Task SaveMatches(TwoPlayerInfo info);
        Task SaveMatches(TeamInfo info);
        Task SaveMatches(ExtremeIndividualInfo info);
        Task SaveMatches(QualificationGrid grid);
    }
}