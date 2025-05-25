using DB.SportHive.Domain;

namespace SportHive.Services.Interfaces
{
    public interface IEnterSportMove
    {
        Task SetFoul(PlayerMovesDto foul);
        Task SetTwoPlayersMove(TwoPlayerMoveDto playerMoveDto);
        Task SetPlayMoves(TwoPlayerMoveDto move);
        Task SetAttackMoves(TwoPlayerMoveDto move);
        Task SetTimeOut(TeamMovesDto teamMoves);
        Task SetTouchdown(TwoPlayerMoveDto move);
    }
}