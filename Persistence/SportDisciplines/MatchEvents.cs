using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class MatchEvents
    {
        public long idEvent { get; set; }
        public long idMatch { get; set; }
        public List<string> composition { get; set; } = new();
        public string NameWinner { get; set; } = null!;
        public string NameLosser { get; set; } = null!;
        public string Draws { get; set; } = null;
    }
}