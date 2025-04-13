using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DB.SportHive.Domain
{
    [Table("UserPhoto")]
    public class UserPhoto
    {
        
        [Column("login")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string login { get; set; } = null!;  
        
        [Column("User")]
        public User User { get; set; } = null!;

        [Column("ProfilePhoto")]
        public string? ProfilePhoto{get;set;}
    }
}
