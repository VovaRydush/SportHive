using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("RugbyStats", RootClass = false)]
    public class RugbyStats : SportStats
    {
        public RugbyStats() { }
        public int Matches { get; set; }
        public int Tries { get; set; }        // Аналог тачдауну
        public int Tackles { get; set; }
        public int PointsScored { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }

}