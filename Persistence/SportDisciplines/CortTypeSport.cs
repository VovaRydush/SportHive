using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace DB.SportHive.MongoDb
{
    public class CortMatches
    {
        public long idMatch { get; }
        public CortTypeSport cortSport { get; set; } = null!;
    }
}