using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DB.SportHive.Domain
{
    [Table("Organization")]
    public class Organization
    {
        
        [Key] 
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }  
        
        [Column("User")]
        public User User { get; set; }  

        [Column("NameOrganization")]
        [Required]
        [MaxLength(256)]
        public string NameOrganization{get;set;}

        [Column("TypeOrganozation")]
        [Required]
        [MaxLength(60)]
        public string TypeOrganozation{get;set;}

        [Column("ProfilePhoto")]
        public string ProfilePhoto{get;set;}

        [Column("Description")]
        public string Description{get;set;}

        [Column("Country")]
        [Required]
        [MaxLength(100)]
        public string Country{get;set;}

        [Column("DateFoundation")]
        public DateTime DateFoundation{get;set;}

    }
}
