namespace DB.SportHive.Domain
{
    public class AmericanFootballStats  : ISportStats
    {
        public int MatchesPlayed { get; set; }
        public int Touchdowns { get; set; }
        public int Yards { get; set; }
        public int Interceptions { get; set; }
        public int Tackles { get; set; }
        public int Wins { get; set; }
        // QB, WR, RB, DL тощо
    }

}