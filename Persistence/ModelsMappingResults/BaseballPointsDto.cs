using DB.SportHive.MongoDb;

namespace DB.SportHive.Domain
{
    public class BaseballPointsDto
    {
        public long IdMatch { get; set; }
        public List<PointBasketball> pointBasketball { get; set; } = null!;
    }
}