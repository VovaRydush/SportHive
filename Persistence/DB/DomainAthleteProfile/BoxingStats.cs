namespace DB.SportHive.Domain
{
    public class BoxingStats
    {
        public int Matches { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Knockouts { get; set; }
        public int RoundsFought { get; set; }
        public double AverageScorePerRound { get; set; }
        public string WeightClass { get; set; } = null!;
        public List<string> Titles { get; set; } = new();
    }

}