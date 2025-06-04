namespace DB.SportHive.Domain
{
    public class BoxingStats : ISportStats
    {
        public int Matches { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Knockouts { get; set; }
        public int RoundsFought { get; set; }
        public double AverageScorePerRound { get; set; }
        public string WeightCategory { get; set; } = null!;
    }

}