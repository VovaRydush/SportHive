using DB.SportHive.Domain;
using DB.SportHive.MongoDb;
using SportHive.Services.Interfaces;

namespace SportHive.Implementations
{
    public class EnterIntermediateData : IEnterIntermediateData
    {
        public Task SetCyclingRace(List<CyclingRace> races)
        {
            throw new NotImplementedException();
        }

        public Task SetDistanceRunning(List<DistanceRunning> runnings)
        {
            throw new NotImplementedException();
        }

        public Task SetPointsBoxStruggleCort(PointsIntBoxStruggle points)
        {
            throw new NotImplementedException();
        }

        public Task SetRowingRace(List<RowingRace> races)
        {
            throw new NotImplementedException();
        }

        public Task SetSwimmingResults(List<AthleteSwimming> swimmings)
        {
            throw new NotImplementedException();
        }

        public Task SetTeamScore(TeamScoreDto score) // тут і подумати над баскетболом
        {
            throw new NotImplementedException();
        }

        public Task SetWeightliftingResults(Weightlifting result)
        {
            throw new NotImplementedException();
        }

        public Task SetWinnerBoxStruggle(BoxWinnerDto winner)
        {
            throw new NotImplementedException();
        }

        public Task SetWinnerChessCheckers(ChessWinnerDto chessWinner)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBaseballMatch(BaseballEvent move)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBaseballPoint()
        {
            throw new NotImplementedException();
        }

        public Task UpdateCheckersMove(CheckersNotationDto notationDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateChessMove(ChessNotationDto notationDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCortSportMatch()
        {
            throw new NotImplementedException();
        }
    }
}