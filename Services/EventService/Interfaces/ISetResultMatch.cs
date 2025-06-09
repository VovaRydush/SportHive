using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface ISetResultMatch
    {
        Task SetResultMatches(long idMatch, string NameWinner, int ScoreEntity1, int ScoreEntity2, int tour);
        Task SetQualificationGrid(long idMatch, string NameWinner, bool isNext);
        Task SetWinnerInMatch(WinnerDto winner);
        Task SetWinnerInMatchStruggle(WinnerDto winner);
        Task SetWinnerInMatchBoard(WinnerDto winner);
    }
}