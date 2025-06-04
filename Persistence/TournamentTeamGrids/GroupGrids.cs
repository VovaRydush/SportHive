namespace DB.SportHive.Domain
{
    public class GroupGrids : TeamIndivGrid
    {
        public int GroupNumber { get; set; }
        public string? NameWinner { get; set; } = null!;

    }
}