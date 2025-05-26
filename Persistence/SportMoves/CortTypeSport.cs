namespace DB.SportHive.MongoDb
{
    public class CortTypeSport
    {
        public TypeTenis type { get; set; }
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 { get; set; } = null!;
        public int Tour { get; set; }
        public int SetCount { get; set; }
        public TimeOut? timeOut { get; set; }
        public List<PlayerFouls> fouls { get; set; } = new();
        public TimeSpan CreatedAt { get; set; }

    }
    public enum TypeTenis
    {
        TableTennis,
        Tenis,
        Badminton
    }
}
