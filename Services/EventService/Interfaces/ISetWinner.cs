using DB.SportHive.Domain;
using DB.SportHive.MongoDb;

namespace SportHive.Services.Interfaces
{
    public interface ISetWinner
    {
        Task SetWinnerChessCheckers(BoardWinner chessWinner);
        Task SetWinnerBoxStruggle(BoxWinnerDto winner);
    }
}