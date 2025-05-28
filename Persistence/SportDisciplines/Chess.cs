using DB.SportHive.Domain;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using StackExchange.Redis;

namespace DB.SportHive.MongoDb
{
    public class ChessMatch : MatchEvents
    {
        public ChessMatch(){}
        public ChessMatch(TwoPlayerInfo info)
        {
            idMatch = info.idMatch;
            Tour = info.tour;
            FullNamePlayer1 = info.FullNamePlayer1;
            FullNamePlayer2 = info.FullNamePlayer2;
        }
        public int Tour{get;set;}
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 { get; set; } = null!;
        public string TimeControl { get; set; } = null!;
        public BoardWinner Result { get; set; } = null!;
        public List<ChessMove> Moves { get; set; } = new();
        public List<PlayerFouls> fouls { get; set; } = new();
    }
    public class BoardWinner
    {
        public long idMatch{ get; set; } 
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer { get; set; } = null!;
        public string typeWin { get; set; } = null!;
        public float countPoints { get; set; }
    }
    public class ChessMove
    {
        public long idMatch { get; set; }
        public string Player { get; set; } = null!;
        public int MoveNumber { get; set; }
        public string MoveNotation { get; set; } = null!;
        public TimeSpan? TimeRemainingWhite { get; set; }
        public TimeSpan? TimeRemainingBlack { get; set; }
    }
}
