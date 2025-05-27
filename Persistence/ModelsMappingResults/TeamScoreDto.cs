namespace DB.SportHive.Domain
{
    public class TeamScoreDto
    { 
        public long idMatch { get; set; }
        public int Score { get; set; }
        public string NameTeam { get; set; } = null!;
    }
}