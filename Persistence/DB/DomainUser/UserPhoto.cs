using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DB.SportHive.Domain
{
    [Table("UserPhoto")]
    public class UserPhoto
    {
        
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }  
        
        [Column("User")]
        public User User { get; set; } = null!;

        [Column("ProfilePhoto")]
        public string? ProfilePhoto{get;set;}
    }
}
