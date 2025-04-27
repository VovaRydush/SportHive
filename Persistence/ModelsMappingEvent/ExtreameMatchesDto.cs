

namespace DB.SportHive.Domain
{
    public class ExtreameMatchesDto
    {
        public string NameEvent { get; set; } = null!;
        public DateTime DataMatch { get; set; }
        public TimeSpan TimeMatch { get; set; }
        public LocationDto location { get; set; } = null!;
        public int tour { get; set; }
        public string AddInformation { get; set; } = null!;
    }
}