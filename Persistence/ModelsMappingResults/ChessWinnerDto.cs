namespace DB.SportHive.Domain
{
    public class ChessWinnerDto
    {
        public long idMatch{ get; set; }
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer { get; set; } = null!;
        public string typeWin { get; set; } = null!;
        public float countPoints { get; set; }
    }
}