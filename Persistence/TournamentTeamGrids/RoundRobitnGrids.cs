namespace DB.SportHive.Domain
{
    public class RoundRobitnGrids : TeamIndivGrid
    {
        public string Result  { get; set; } = null!;
        public string? NameWinner { get; set; } = null!;
    }
}