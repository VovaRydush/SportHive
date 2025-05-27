using DB.SportHive.Domain;
using DB.SportHive.MongoDb;

namespace SportHive.Services.Interfaces
{
    public interface IEnterIntermediateData
    {
        Task UpdateBaseballMatch(BaseballEvent move);
        Task UpdateBaseballPoint();
        Task UpdateCheckersMove(CheckersNotationDto notationDto);
        Task UpdateChessMove(ChessNotationDto notationDto);
        Task SetWinnerChessCheckers(ChessWinnerDto chessWinner);
        Task SetPointsBoxStruggleCort(PointsIntBoxStruggle points); // бокс, бородьба та всі види спорту з кортом
        Task SetWinnerBoxStruggle(BoxWinnerDto winner);
        Task SetCyclingRace(List<CyclingRace> races);
        Task SetDistanceRunning(List<DistanceRunning> runnings);
        Task SetRowingRace(List<RowingRace> races);
        Task SetSwimmingResults(List<AthleteSwimming> swimmings);
        Task SetWeightliftingResults(Weightlifting result);
        Task SetTeamScore(TeamScoreDto score);
    }
}