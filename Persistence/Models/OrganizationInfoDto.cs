using Microsoft.AspNetCore.Http;

namespace DB.SportHive.Domain
{
    public class OrganizationInfoDto{
        public string NameOrganization{get;set;}
        public string TypeOrganozation{get;set;}
        public string Email{get;set;}
        public string Country{get;set;}
        public IFormFile ProfilePhoto{get;set;}
        public string Description{get;set;} = null!;
      
    } 
}