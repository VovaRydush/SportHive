namespace DB.SportHive.Domain
{
    public class ChessNotationDto
    {
        public long idMatch { get; set; }
        public string Player { get; set; } = null!;
        public int MoveNumber { get; set; }
        public string MoveNotation { get; set; } = null!;
        public TimeSpan? TimeRemainingWhite { get; set; }
        public TimeSpan? TimeRemainingBlack { get; set; }
    }
}