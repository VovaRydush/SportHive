namespace DB.SportHive.Domain
{
    public class WeightliftingStats
    {
        public string WeightClass { get; set; } = null!;
        public int BestSnatchKg { get; set; }
        public int BestCleanAndJerkKg { get; set; }
        public int TotalKg => BestSnatchKg + BestCleanAndJerkKg;
        public List<string> Competitions { get; set; } = new();
    }
}
