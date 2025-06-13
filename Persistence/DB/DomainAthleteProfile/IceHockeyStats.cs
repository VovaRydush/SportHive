using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("IceHockeyStats", RootClass = false)]
    public class IceHockeyStats : SportStats
    {
        public IceHockeyStats() { }
        public int Matches { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Draws { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

    }
}