using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.Domain
{
    [BsonDiscriminator("FootballStats", RootClass = false)]
    public class FootballStats : SportStats
    {
        public FootballStats()
        {
        }
        public FootballStats(string fullName, string Login, string sportType)
        {
        }

        public int Matches { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        public int Draws { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}