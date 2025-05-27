namespace DB.SportHive.Domain
{
    public class TeamMovesDto
    {
        public long idMatch { get; set; }
        public string NameTeam { get; set; } = null!;
        public string TimeStartTimeOut { get; set; } = null!;
        public string TimeEndTimeOut { get; set; } = null!;
        public string NameSport { get; set; } = null!;
    }
}