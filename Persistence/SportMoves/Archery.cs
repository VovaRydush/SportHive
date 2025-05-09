namespace DB.SportHive.MongoDb
{
    public class Archery
    {
        public long idMatch { get; set; }
        public string CompetitionType { get; set; } = null!;
        public string FullNamePlayer { get; set; } = null!;
        public string BowType { get; set; } = null!;
        public float Distance { get; set; }
        public Foul foul { get; set; }
    }
}
