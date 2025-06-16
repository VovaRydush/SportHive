using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.MongoDb
{
    [BsonIgnoreExtraElements]
    public class MatchEvents
    {
        public long idEvent { get; set; }
        public long idMatch { get; set; }
        public List<string> composition { get; set; } = new();
        public string firstTeamScore { get; set; } = null!;
        public string secondTeamScore { get; set; } = null!;
        public string NameWinner { get; set; } = null!;
        public string NameLosser { get; set; } = null!;
        public List<string> Draws { get; set; } = null;
    }
}