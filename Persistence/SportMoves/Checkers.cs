namespace DB.SportHive.MongoDb
{
    public class CheckersGame
    {
        public long IdMatch { get; set; }
        public string WhitePlayer { get; set; } = null!;
        public string BlackPlayer { get; set; } = null!;
        public string Result { get; set; } = null!; // "1-0", "0-1", "½-½"
        public string TimeControl { get; set; } = null!; // "15+3"
        public List<CheckersMove> Moves { get; set; } = new();
        public List<Foul> Fouls { get; set; } = new();
        public DateTime Date { get; set; }
    }

    public class CheckersMove
    {
        public int MoveNumber { get; set; }
        public string Notation { get; set; } = null!; // "12-16", "14x23", "28-21 (K)"
        public bool IsCapture { get; set; }
        public bool IsKingMove { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
    }
}
