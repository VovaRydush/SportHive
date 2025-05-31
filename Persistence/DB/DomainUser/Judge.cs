using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DB.SportHive.Domain
{
    [Table("Judge")]
    public class Judge
    {

        [Column("User")]
        public User User { get; set; } = null!;

        [Column("Login")]
        [Key]
        public string login { get; set; } = null!;

        [Column("FirsName")]
        [Required]
        [MaxLength(60)]
        public string FirsName { get; set; } = null!;

        [Column("LastName")]
        [Required]
        [MaxLength(60)]
        public string LastName { get; set; } = null!;

        [Column("DataBirth")]
        public DateTime DataBirth { get; set; }

        [Column("Organization")]
        public List<OrganizationJudge> OrganizationJudge { get; set; } = null!;
        public ICollection<ExtremeMatch> ExtremeMatch { get; set; } = new List<ExtremeMatch>();
        public ICollection<IndividualMatch> IndividualMatch { get; set; } = new List<IndividualMatch>();
        public ICollection<TeamMatch> TeamMatch { get; set; } = new List<TeamMatch>();
    }
}
