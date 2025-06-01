namespace DB.SportHive.Domain
{
    public class CheckersChessStats : ISportStats
    {
        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int TournamentsPlayed { get; set; }
        public List<string> TournamentMedals { get; set; } = new();
        public int EloRating { get; set; }
        public int FastestWinMoves { get; set; }         // Найменша кількість ходів до перемоги
        public double AverageMoveTimeSeconds { get; set; }
        public string PreferredTimeControl { get; set; } = null!; // Наприклад: "Classic", "Rapid", "Blitz"
    }

}