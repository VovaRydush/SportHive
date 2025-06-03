namespace DB.SportHive.Domain
{
    public class AthletesTeamDto
    {
        public string? NameTeam { get; set; } = null!;
        public long idMatch{ get; set; }
        public string loginPlayer { get; set; } = null!;
        
    }
}