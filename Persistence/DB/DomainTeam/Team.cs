using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.SportHive.Domain
{
    [Table("Team")]
    public class Team
    {

        [Required]
        [Column("TeamName")]
        public string TeamName { get; set; } = null!;

        [Required]
        [Column]
        public string TeamPhoto {get;set;} = null!;

        [Required]
        [Column("LoginTrainer")]
        public string LoginTrainer { get; set; } = null!;

        public Trainer Trainer { get; set; } = null!;

        public List<TeamAthlete> TeamAthletes { get; set; } = new();

        [Required]
        [Column("TypeSport")]
        [MaxLength(100)]
        public string TypeSport { get; set; } = null!;

        [Column("Organization")]
        public List<OrganizationTeam>? OrganizationTeam{get;set;} = null!;
    }

}
