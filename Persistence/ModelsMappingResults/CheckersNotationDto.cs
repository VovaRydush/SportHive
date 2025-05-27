namespace DB.SportHive.Domain
{
    public class CheckersNotationDto
    {
        public long idMatch { get; set; }
        public int MoveNumber { get; set; }
        public string Notation { get; set; } = null!; // "12-16", "14x23", "28-21 (K)"
        public bool IsCapture { get; set; }
        public bool IsKingMove { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
    }
}