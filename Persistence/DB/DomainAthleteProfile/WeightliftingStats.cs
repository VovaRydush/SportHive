namespace DB.SportHive.Domain
{
    public class WeightliftingStats : SportStats
    {
        public string WeightCategory { get; set; } = null!;
        public int BestSnatchKg { get; set; }
        public int BestCleanAndJerkKg { get; set; }
        public int TotalKg => BestSnatchKg + BestCleanAndJerkKg; 
        
    }
}
