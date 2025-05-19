namespace DB.SportHive.Domain
{
    public class TwoPlayerInfo
    {
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 { get; set; } = null!;
        public string NameDesipline { get; set; } = null!;
        public long idMatch { get; set; }
    }
}