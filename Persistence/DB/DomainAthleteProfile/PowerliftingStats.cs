namespace DB.SportHive.Domain
{
    public class PowerliftingStats
    {
        public string WeightClass { get; set; } = null!;
        public int BestSquatKg { get; set; }
        public int BestBenchPressKg { get; set; }
        public int BestDeadliftKg { get; set; }
        public int TotalKg => BestSquatKg + BestBenchPressKg + BestDeadliftKg;
        public List<string> Medals { get; set; } = new();
    }
}