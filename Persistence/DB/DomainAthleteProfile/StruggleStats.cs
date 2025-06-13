namespace DB.SportHive.Domain
{
    public class StruggleStats : SportStats
    {
        public int Matches { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int TechnicalWins { get; set; }   // За технічними балами
        public int PinWins { get; set; }         // Утримання
        public string WeightCategory { get; set; } = null!;
    }
}