using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DB.SportHive.Domain
{
    [Table("Judge")]
    public class Judge
    {
        
        [Column("User")]
        public User User { get; set; }  
       
        [Key] 
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } 
        
        [Column("FirsName")]
        [Required]
        [MaxLength(60)]
        public string FirsName{get;set;}

        [Column("LastName")]
        [Required]
        [MaxLength(60)]
        public string LastName{get;set;}

        [Column("DataBirth")]
        public DateTime DataBirth{get;set;}

        [Column("Organization")]
        public List<OrganizationJudge> OrganizationJudge{get;set;}

    }
}
