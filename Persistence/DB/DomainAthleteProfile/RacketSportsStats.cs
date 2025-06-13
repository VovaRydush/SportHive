using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("RacketSportsStats", RootClass = false)]
    public class RacketSportsStats : SportStats
    {
        public RacketSportsStats()
        {
        }
        public int Matches { get; set; }
        public int Losses { get; set; }
        public int Wins { get; set; }
    }
}