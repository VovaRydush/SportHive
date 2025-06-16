using StackExchange.Redis;

namespace DB.SportHive.Domain
{
    public class TokenRole
    {
        public string role { get; set; } = null!;
        public string token { get; set; } = null!; 
    }
}