using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DB.SportHive.Domain
{
    [Table("Organization")]
    public class Organization
    {

        [Column("Login")]
        [Key]
        public string login { get; set; } = null!;

        [Column("User")]
        public User User { get; set; } = null!;

        [Column("NameOrganization")]
        [Required]
        [MaxLength(256)]
        public string NameOrganization { get; set; } = null!;

        [Column("TypeOrganozation")]
        [Required]
        [MaxLength(60)]
        public string TypeOrganozation { get; set; } = null!;

        [Column("Description")]
        public string Description { get; set; } = null!;

        [Column("Country")]
        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = null!;

        [Column("DateFoundation")]
        public DateTime DateFoundation { get; set; }
        [Column("Judges")]
        public List<OrganizationJudge> OrganizationJudge { get; set; } = null!;

    }
}
