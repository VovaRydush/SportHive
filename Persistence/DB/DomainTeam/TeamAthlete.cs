using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.SportHive.Domain
{
    [Table("TeamAthlete")]
    public class TeamAthlete
    {
        [Column("IdTeam")]
        public long IdTeam { get; set; }

        [ForeignKey("IdTeam")]
        public Team Team { get; set; }=null!;

        [Column("IdAthlete")]
        public long IdAthlete { get; set; }

        public Athlete Athlete { get; set; }=null!;
        [MaxLength(100)]
        public string AthleteStatus { get; set; }=null!;
    }
}
