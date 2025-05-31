namespace DB.SportHive.Domain
{
    public class TwoPlayerInfo
    {
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 { get; set; } = null!;
        public string NameDesipline { get; set; } = null!;
        public int tour { get; set; }
        public long idMatch { get; set; }
        public long idEvent { get; set; }
    }
}