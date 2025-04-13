using System.ComponentModel.DataAnnotations.Schema;

namespace DB.SportHive.Domain
{
    [Table("OrganizationTeam")]
    public class OrganizationTeam
    {
         
        [Column("LoginOrganization")]
        public string LoginOrganization { get; set; } = null!;
        
        [Column("NameComand")]
        public string NameComand { get; set; } = null!;

        [Column("Organization")]
        public Organization Organization {get;set;} = null!;

        [Column("Team")]
        public Team Team{get;set;} = null!;
    }
}
