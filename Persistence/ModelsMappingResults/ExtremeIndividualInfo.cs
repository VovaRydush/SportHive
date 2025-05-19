using StackExchange.Redis;

namespace DB.SportHive.Domain
{
    public class ExtremeIndividualInfo
    {
        public long idMatch;
        public int tour;
        public string NameDesipline { get; set; } = null!;
        public List<PlayerInfo> playersName { get; set; } = null!;
    }
    public class PlayerInfo
    {
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer { get; set; } = null!;
    }
}