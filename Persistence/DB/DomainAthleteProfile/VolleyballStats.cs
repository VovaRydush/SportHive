using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("VolleyballStats", RootClass = false)]
    public class VolleyballStats : SportStats
    {
        public VolleyballStats() { }
        public int Matches { get; set; }
        public int Blocks { get; set; }
        public int Errors { get; set; }
        public int Win { get; set; }
        public int Losses { get; set; }
    }
}
