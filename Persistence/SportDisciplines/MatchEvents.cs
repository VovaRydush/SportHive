using MongoDB.Bson.Serialization.Attributes;

namespace DB.SportHive.MongoDb
{
    [BsonIgnoreExtraElements]
    public class MatchEvents
    {
        public long idEvent { get; set; }
        public long idMatch { get; set; }
        public List<string> composition { get; set; } = new(); //тут команди 1 і 2 типу
        public string firstTeamScore { get; set; } = null!;
        public string secondTeamScore { get; set; } = null!;
        public string NameWinner { get; set; } = null!;
        public string NameLosser { get; set; } = null!;
        public List<string> Draws { get; set; } = null;
    }
}