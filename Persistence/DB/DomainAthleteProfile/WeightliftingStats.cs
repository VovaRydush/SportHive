using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("WeightliftingStats", RootClass = false)]
    public class WeightliftingStats : SportStats
    {
        public WeightliftingStats() { }
        public string WeightCategory { get; set; } = null!;
        public int BestSnatchKg { get; set; }
        public int BestCleanAndJerkKg { get; set; }
        public int TotalKg => BestSnatchKg + BestCleanAndJerkKg;

    }
}
