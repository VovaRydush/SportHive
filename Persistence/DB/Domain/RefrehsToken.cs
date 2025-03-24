using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DB.SportHive.Domain
{
    [Table("RefreshToken")]
    public class RefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("token")]
        public string refreshToken { get; set; }

        [Column("expires")]
        public DateTime Expires { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Column("user_id")] 
        public long UserId { get; set; }
    }
}
