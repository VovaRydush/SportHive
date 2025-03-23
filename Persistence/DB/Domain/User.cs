using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DB.SportHive.Domain
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public long Id { get; set; }  
        
        [Required]
        [Column("mail")] 
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }
        
        [Required]
        [Column("HashPassword")] 
        public string HashPassword{get;set;}
        public UserdDtails UserdDtails { get; set; }

        [Column("token")] 
        public string RefreshToken { get; set; }
    }
}
