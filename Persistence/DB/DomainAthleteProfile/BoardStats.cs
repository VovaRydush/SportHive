using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("BoardStats", RootClass = false)]
    public class BoardStats : SportStats
    {
        public BoardStats() { }
        public int Matches { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int EloRating { get; set; }
        public List<string> Tournaments { get; set; } = null!;
    }
}
