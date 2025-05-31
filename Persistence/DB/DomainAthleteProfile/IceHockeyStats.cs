namespace DB.SportHive.Domain
{
    public class IceHockeyStats
    {
        public int MatchesPlayed { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int PenaltyMinutes { get; set; }
        public int Wins { get; set; }
        public string Position { get; set; } = null!; // "Нападник", "Захисник", "Воротар"

        // Для воротарів
        public int Saves { get; set; }
        public double SavePercentage { get; set; }
    }
}