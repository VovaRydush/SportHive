using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("BoxingStats", RootClass = false)]
    public class BoxingStats : SportStats
    {
        public BoxingStats() { }
        public int Matches { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Knockouts { get; set; }
        public int RoundsFought { get; set; }
        public double AverageScorePerRound { get; set; }
        public string WeightCategory { get; set; } = null!;
    }

}