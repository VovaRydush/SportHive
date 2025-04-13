using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.SportHive.Domain
{
    [Table("Team")]
    public class Team
    {
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column("TeamName")]
        public string TeamName { get; set; }=null!;

        [Required]
        [Column("IdTraine")]
        public long IdTraine { get; set; }

        public Trainer Trainer { get; set; }=null!;

        public List<TeamAthlete> TeamAthletes { get; set; } = new();

        [Required]
        [Column("TypeSport")]
        [MaxLength(100)]
        public string TypeSport { get; set; }=null!;
    }
}
