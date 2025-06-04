namespace DB.SportHive.Domain
{
    public class RacketSportsStats : ISportStats
    {
        public RacketSportsStats()
        {
        }
        public int Matches { get; set; }
        public int Losses { get; set; }
        public int Wins { get; set; }
    }
}