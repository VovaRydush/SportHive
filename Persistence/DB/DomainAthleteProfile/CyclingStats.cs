using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("CyclingStats", RootClass = false)]
    public class CyclingStats : SportStats
    {
        public CyclingStats() { }
        public int Matches { get; set; }
        public double BestTime { get; set; }            // у секундах
        public string BestDistance { get; set; } = null!;       // "10km", "40km", "Time Trial"
        public double AverageSpeedKmH { get; set; }     // середня швидкість
        public int Wins { get; set; }
        public List<string> RaceTypes { get; set; } = null!;     // "Road", "Track", "MTB", "BMX"
    }

}