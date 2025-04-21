using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.SportHive.Domain
{
    [Table("TeamAthlete")]
    public class TeamAthlete
    {
        [Column("NameTeam")]
        public string NameTeam { get; set; } = null!;

        [ForeignKey("IdTeam")]
        public Team Team { get; set; }=null!;

        [Column("IdAthlete")]
        public string  loginAthlets { get; set; } =  null!;

        public Athlete Athlete { get; set; }=null!;
        
        [MaxLength(100)]
        public string AthleteStatus { get; set; }=null!;
    }
}
