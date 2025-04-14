using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace DB.SportHive.Domain
{
    [Table("OrganizationTrainer")]
    public class OrganizationTrainer
    {
         
        [Column("LoginOrganization")]
        public string LoginOrganization { get; set; } = null!;
        
        [Column("LoginTraine")]
        public string LoginTraine { get; set; } = null!;

        [Column("Organization")]
        public Organization Organization {get;set;} = null!;

        [Column("Trainer")]
        public Trainer Trainer{get;set;} = null!;
    }
}
