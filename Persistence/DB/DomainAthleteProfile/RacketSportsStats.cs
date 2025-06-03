namespace DB.SportHive.Domain
{
    public class RacketSportsStats : ISportStats
    {
        public RacketSportsStats()
        {
        }

        public int Matches { get; set; }
        public int Losses { get; set; }
        public int Wins { get; set; }
        public int Aces { get; set; }              // Подачі напряму в очко
        public int DoubleFaults { get; set; }      // Подвійні помилки
        public int TotalPointsWon { get; set; }
        public string DominantHand { get; set; } = null!;  // "Right" або "Left"
        public List<string> TournamentMedals { get; set; } = new();
    }

}