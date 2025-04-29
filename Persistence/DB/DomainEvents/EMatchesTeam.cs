using System.ComponentModel.DataAnnotations.Schema;


namespace DB.SportHive.Domain
{
    [Table("EMatchesTeam")]
    public class EMatchesTeam
    {
        public string NameTeam { get; set; } = null!;
        public long IdExtremeMatches { get; set; }
        public ExtremeMatch ExtremeMatch { get; set; } = null!;
        public Team Team { get; set; } = null!;
        
    }
}
