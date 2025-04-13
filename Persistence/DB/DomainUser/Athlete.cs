using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DB.SportHive.Domain
{
    [Table("Athlete")]
    public class Athlete
    {
        
        [Column("User")]
        public User User { get; set; } = null!;
       
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("FirsName")]
        [Required]
        [MaxLength(60)]
        public string FirsName{get;set;}=null!;

        [Column("LastName")]
        [Required]
        [MaxLength(60)]
        public string LastName{get;set;}=null!;

        [Column("DataBirth")]
        public DateTime DataBirth{get;set;}

        [Required]
        [Column("TypeSport")]
        [MaxLength(100)]
        public string TypeSport{get;set;}=null!;

        public List<TeamAthlete>? TeamAthletes { get; set; }

    }
}
