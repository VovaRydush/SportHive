using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("BasketballStats", RootClass = false)]
    public class BasketballStats : SportStats
    {
        public BasketballStats() { }
        public int Matches { get; set; }
        public int Losses { get; set; }
        public int Wins { get; set; }
        public int Points { get; set; }
        public int Rebounds { get; set; }
        public int Assists { get; set; }
        public int Blocks { get; set; }
        public double ThreePointPercentage { get; set; }
        public double FreeThrowPercentage { get; set; }
    }
}
