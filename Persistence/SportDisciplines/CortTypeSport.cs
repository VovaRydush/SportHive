using DB.SportHive.Domain;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace DB.SportHive.MongoDb
{
    public class CortMatches : MatchEvents
    {
        public CortMatches(){}
        public CortMatches(TwoPlayerInfo info)
        {
            idMatch = info.idMatch;
            idEvent = info.idEvent;
            Tour = info.tour;
            FullNamePlayer1 = info.FullNamePlayer1;
            FullNamePlayer2 = info.FullNamePlayer2;
            composition.Add(info.FullNamePlayer1);
            composition.Add(info.FullNamePlayer2);
        }
        public TypeTenis type { get; set; }
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 { get; set; } = null!;
        public int Tour { get; set; }
        public int SetCount { get; set; }
        public TimeOut? timeOut { get; set; }
        public List<RoundPoints> points { get; set; } = null!;
        public List<PlayerFouls> fouls { get; set; } = new();
        public TimeSpan CreatedAt { get; set; }
    }
}