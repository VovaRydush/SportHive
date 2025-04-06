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
        [MaxLength(255)]
        [Column("HashPassword")] 
        public string HashPassword{get;set;}

        [Required]
        [Column("EmailConfirmed")]
        public bool isEmailConfirmed {get;set;}

        [Required]
        [MaxLength(512)]
        [Column("refreshToken")]
        public string refreshToken{get;set;}

        [Required]
        [Column("Role")]
        public string Role {get;set;}

    }
}
