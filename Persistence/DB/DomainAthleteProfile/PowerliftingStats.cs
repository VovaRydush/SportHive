using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("PowerliftingStats", RootClass = false)]
    public class PowerliftingStats : SportStats
    {
        public PowerliftingStats() { }
        public string WeightCategory { get; set; } = null!;
        public int BestSquatKg { get; set; }
        public int BestBenchPressKg { get; set; }
        public int BestDeadliftKg { get; set; }
        public int TotalKg => BestSquatKg + BestBenchPressKg + BestDeadliftKg;
    }
}