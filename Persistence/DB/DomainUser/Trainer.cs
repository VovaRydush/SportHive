using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DB.SportHive.Domain
{
    [Table("Trainer")]
    public class Trainer
    {

        [Column("Login")]
        [Key]
        public string login { get; set; } = null!;

        [Column("User")]
        public User User { get; set; } = null!;

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

        public List<Team>? Teams { get; set; }
        public List<OrganizationTrainer>? OrganizationTrainer{get;set;} = null!;

    }
}
