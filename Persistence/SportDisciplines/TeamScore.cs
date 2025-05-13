namespace DB.SportHive.MongoDb
{
    public class TeamScore
    {
        public int Score { get; set; }
        public string NameTeam { get; set; } = null!;
        public PointBasketball? pointBasketball { get; set; }
    }
}