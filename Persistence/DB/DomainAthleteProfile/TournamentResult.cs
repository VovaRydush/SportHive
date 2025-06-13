using Confluent.Kafka;

namespace DB.SportHive.Domain
{
    
    public class TournamentResult
    {
        public TournamentResult() { }
        public string EventName { get; set; } = null!;
        public string Placement { get; set; } = null!;// 1 місце, 2 місце, "Участь"
        public DateTime Date { get; set; }
    }
}
