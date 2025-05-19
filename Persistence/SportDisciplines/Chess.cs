using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class ChessMatch : MatchEvents
    {
         public ChessMatch(TwoPlayerInfo info)
        {
            idMatch = info.idMatch;
            FullNamePlayer1 = info.FullNamePlayer1;
            FullNamePlayer2 = info.FullNamePlayer2;
        }
        public int Tour{get;set;}
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 { get; set; } = null!;
        public string TimeControl { get; set; } = null!;
        public string Result { get; set; } = null!;
        public List<ChessMove> Moves { get; set; } = new();
        public List<Foul> Fouls { get; set; } = new();
        public DateTime Date { get; set; }
    }

    public class ChessMove
    {
        public int MoveNumber { get; set; }
        public string MoveNotation { get; set; } = null!;
        public TimeSpan? TimeRemainingWhite { get; set; }
        public TimeSpan? TimeRemainingBlack { get; set; }
    }
}
