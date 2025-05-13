namespace DB.SportHive.MongoDb
{
    public class CortTypeSport
    {
        public TypeTenis type { get; set; }
        public int SetCount { get; set; }
        public TimeOut? timeOut { get; set; }
        public List<PlayerFouls> playerFouls { get; set; } = null!;
        public TimeSpan CreatedAt { get; set; }

    }
    public enum TypeTenis
    {
        TableTennis,
        Tenis,
        Badminton
    }
}
