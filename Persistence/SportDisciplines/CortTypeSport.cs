using DB.SportHive.Domain;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace DB.SportHive.MongoDb
{
    public class CortMatches : MatchEvents
    {
        public CortMatches(TwoPlayerInfo info)
        {
            idMatch = info.idMatch;
            cortSport.Tour = info.tour;
            cortSport.FullNamePlayer1 = info.FullNamePlayer1;
            cortSport.FullNamePlayer2 = info.FullNamePlayer2;
        }
        public CortTypeSport cortSport { get; set; } = null!;
    }
}