namespace DB.SportHive.MongoDb
{
    public class ChessGame
    {
        public long IdMatch { get; set; }
        public string WhitePlayer { get; set; } = null!;
        public string BlackPlayer { get; set; } = null!;
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
