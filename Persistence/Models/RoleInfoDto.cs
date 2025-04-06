using Microsoft.AspNetCore.Http;

namespace DB.SportHive.Domain
{
    public class RoleInfoDto{
        public string FistName{get;set;}
        public string LastName{get;set;}
        public string Email{get;set;}
        public IFormFile ProfilePhoto{get;set;}
        public string TypeSport{get;set;} = null!;
      
    } 
}