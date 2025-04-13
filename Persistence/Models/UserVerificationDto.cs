namespace DB.SportHive.Domain
{
    public class UserVerificationDto{
        public string Email{get;set;} = null!;
        public string Code{get;set;} = null!;
    } 
}