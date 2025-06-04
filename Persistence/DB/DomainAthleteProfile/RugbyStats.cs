namespace DB.SportHive.Domain
{
    public class RugbyStats : ISportStats
    {
        public int Matches { get; set; }
        public int Tries { get; set; }        // Аналог тачдауну
        public int Tackles { get; set; }
        public int PointsScored { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }

}