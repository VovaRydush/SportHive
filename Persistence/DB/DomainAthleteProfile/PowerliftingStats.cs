namespace DB.SportHive.Domain
{
    public class PowerliftingStats : SportStats
    {
        public string WeightCategory { get; set; } = null!;
        public int BestSquatKg { get; set; }
        public int BestBenchPressKg { get; set; }
        public int BestDeadliftKg { get; set; }
        public int TotalKg => BestSquatKg + BestBenchPressKg + BestDeadliftKg;
    }
}