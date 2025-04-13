using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DB.SportHive.Domain
{
    [Table("OrganizationJudge")]
    public class OrganizationJudge
    {
         
        [Column("IdOrganization")]
        public long IdOrganization { get; set; }  
        
        [Column("IdJudge")]
        public long IdJudge { get; set; }  

        [Column("Organization")]
        public Organization Organization {get;set;}

        [Column("Judge")]
        public Judge Judge{get;set;}
    }
}
