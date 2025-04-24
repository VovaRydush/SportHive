using System.ComponentModel.DataAnnotations.Schema;


namespace DB.SportHive.Domain
{
    [Table("EMatchesAthlete")]
    public class EMatchesAthlete
    {
        public string loginAthlete { get; set; } = null!;
        public long IdExtremeMatches { get; set; }
        public ExtremeMatch ExtremeMatch { get; set; } = null!;
        public Athlete Athlete { get; set; } = null!;
        
    }
}
