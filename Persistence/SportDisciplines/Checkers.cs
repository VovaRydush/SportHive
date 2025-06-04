using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class CheckersGame : MatchEvents
    {
        public CheckersGame(){}
        public CheckersGame(TwoPlayerInfo info)
        {
            idMatch = info.idMatch;
            idEvent = info.idEvent;
            Tour = info.tour;
            FullNamePlayer1 = info.FullNamePlayer1;
            FullNamePlayer2 = info.FullNamePlayer2;
            composition.Add(info.FullNamePlayer1);
            composition.Add(info.FullNamePlayer2);
        }
        public int Tour{get;set;}
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 { get; set; } = null!;
        public BoardWinner Result { get; set; } = null!; // "1-0", "0-1", "½-½"
        public string TimeControl { get; set; } = null!; // "15+3"
        public List<CheckersMove> Moves { get; set; } = new();
        public List<PlayerFouls> fouls { get; set; } = new();
        public DateTime Date { get; set; }
    }

    public class CheckersMove
    {
        public long idMatch { get; set; }
        public int MoveNumber { get; set; }
        public string MoveNotation { get; set; } = null!; // "12-16", "14x23", "28-21 (K)"
        public bool IsCapture { get; set; }
        public bool IsKingMove { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
    }
}
