namespace DB.SportHive.Domain
{
    public class SwissSystemGrid : TeamIndivGrid
    {
        public int ratingElo1 { get; set; }
        public int ratingElo2 { get; set; }
        public string NameWinner { get; set; } = null!;
    }
    public class SwissSystemPlayed
    {
        public string NameFirstEntity { get; set; } = null!;
        public string NameSecondEntity { get; set; } = null!;
    }
}