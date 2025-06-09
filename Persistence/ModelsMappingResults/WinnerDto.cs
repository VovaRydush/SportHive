namespace DB.SportHive.Domain
{
    public class WinnerDto
    {
        public long idMatch { get; set; }
        public string FullNamePlayer { get; set; } = null!;
        public int? round { get; set; }
        public string loginWinner { get; set; } = null!;
        public float? countPoints { get; set; }
        public string win { get; set; } = null!;
    }
}