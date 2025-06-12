namespace DB.SportHive.Domain
{
    public class IceHockeyStats : ISportStats
    {
        public int Matches { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Draws { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

    }
}