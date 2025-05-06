using Microsoft.AspNetCore.Http;

namespace DB.SportHive.Domain
{
    public class EventDto
    {
        public string NameEvent { get; set; } = null!;
        public SelectionSystems systems { get; set; }
        public IFormFile EventPhoto { get; set; } = null!;
        public DateTime DataStart { get; set; }
        public DateTime DataEnd { get; set; }
        public string TypeSport{get;set;} = null!;
        public string description { get; set; } = null!;

    }
}