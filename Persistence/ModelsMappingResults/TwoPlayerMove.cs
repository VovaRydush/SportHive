using StackExchange.Redis;

namespace DB.SportHive.Domain
{
    public class TwoPlayerMoveDto
    {
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 { get; set; } = null!;
        public int yards { get; set; }
        public string NameSport { get; set; } = null!;
        public bool realization { get; set; }
        public string timeMove { get; set; } = null!;
        public string typeMove { get; set; } = null!;
        public long idMatch { get; set; }
    }
}