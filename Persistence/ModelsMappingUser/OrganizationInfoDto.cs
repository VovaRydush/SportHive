using Microsoft.AspNetCore.Http;

namespace DB.SportHive.Domain
{
    public class OrganizationInfoDto
    {
        public string NameOrganization{get;set;}  = null!;
        public string TypeOrganozation{get;set;} = null!;
        public string Email{get;set;} = null!;
        public string Country{get;set;} = null!;
        public IFormFile ProfilePhoto{get;set;} = null!;
        public string Description{get;set;} = null!;
    } 
}