using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("AmericanFootballStats", RootClass = false)]
    public class AmericanFootballStats : SportStats
    {
        public AmericanFootballStats() { }
        public int Matches { get; set; }
        public int Touchdowns { get; set; }
        public int Interceptions { get; set; }
        public int Tackles { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
    }

}