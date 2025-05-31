namespace DB.SportHive.Domain
{
    public class AmericanFootballStats
    {
        public int MatchesPlayed { get; set; }
        public int Touchdowns { get; set; }
        public int Yards { get; set; }
        public int Interceptions { get; set; }
        public int Tackles { get; set; }
        public int Wins { get; set; }
        public string Position { get; set; } = null!; // QB, WR, RB, DL тощо
    }

}