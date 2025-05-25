namespace DB.SportHive.Domain
{
    public class PlayerMovesDto
    {
        public long idMatch { get; set; }
        public string FullNamePlayer { get; set; } = null!; // фол, ігорові моменти, атака, тачдаун
        public string login { get; set; } = null!;
        public TimeSpan timeMove { get; set; }
        public string typeMove { get; set; } = null!;
        public string? realization { get; set; } = null!;
        public int? yards{ get; set; }
    }
}