using Microsoft.AspNetCore.Http;

namespace DB.SportHive.Domain
{
    public class RoleInfoDto
    {
        public string FistName{get;set;} = null!;
        public string LastName{get;set;} = null!;
        public string Login{get;set;} = null!;
        public DateTime dateBirhsday{get;set;}
        public IFormFile ProfilePhoto { get; set; } = null!;
        public string TypeSport{get;set;} = null!;
    } 
}