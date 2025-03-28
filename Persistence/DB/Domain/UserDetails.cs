using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.SportHive.Domain
{
    [Table("user_details")]
    public class UserdDtails
    {
        [Key]  
        public long UserId { get; set; }  

        
        [ForeignKey("UserId")]
        public User User { get; set; }  

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePhoto { get; set; }
        public DateTime DataBirhsday { get; set; }
        public Status Status { get; set; }
        public Role Role { get; set; }
    }

    
    public enum Status
    {
        active,
        noactive,
        spare
    }

   
    public enum Role
    {
        athlete,
        coach
    }
}
