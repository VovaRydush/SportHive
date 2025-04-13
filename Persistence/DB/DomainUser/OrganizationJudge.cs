using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DB.SportHive.Domain
{
    [Table("OrganizationJudge")]
    public class OrganizationJudge
    {
         
        [Column("LoginOrganization")]
        public string LoginOrganization { get; set; } = null!;
        
        [Column("LoginJudge")]
        public string LoginJudge { get; set; } = null!;

        [Column("Organization")]
        public Organization Organization {get;set;} = null!;

        [Column("Judge")]
        public Judge Judge{get;set;} = null!;
    }
}
