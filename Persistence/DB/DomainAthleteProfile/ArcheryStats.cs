using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("ArcheryStats", RootClass = false)]
    public class ArcheryStats : SportStats
    {
        public ArcheryStats() { }
        public int Competitions { get; set; }
        public int Matches { get; set; }
        public int MaxPointsPerRound { get; set; }
        public double AveragePointsPerRound { get; set; }
        public int Bullseyes { get; set; }            // Пряме влучання в центр
        public string DistanceType { get; set; } = null!;     // Напр: "30m", "70m", "Olympic"
    }

}