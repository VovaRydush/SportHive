using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace DB.SportHive.Domain
{
    public class LocationDto
    {
        public decimal lat { get; set; }
        public decimal lng { get; set; }
        public string place_id { get; set; } = null!;
        public string address { get; set; }
    }
}