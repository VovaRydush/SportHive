using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("RowingStats", RootClass = false)]
    public class RowingStats : SportStats
    {
        public RowingStats() { }
        public int Matches { get; set; }
        public string BoatType { get; set; } = null!;           // "Single", "Double", "Four", "Eight"
        public string Discipline { get; set; } = null!;
        public double BestTimeSeconds { get; set; }     // У секундах
        public string BestDistance { get; set; } = null!;         // Наприклад, "2000m"
    }
}