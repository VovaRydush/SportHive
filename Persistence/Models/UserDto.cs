namespace DB.SportHive.Domain
{
    public class UserInfoDto{
        public string Email{get;set;} = null!;
        public string Password{get;set;} = null!;
        public string? Role{get;set;} = null!;
      
    } 
}