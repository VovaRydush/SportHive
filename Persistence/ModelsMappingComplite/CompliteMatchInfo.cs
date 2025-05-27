namespace DB.SportHive.Domain
{
    public class CompliteMatchInfo
    {
        public long idMatch { get; set; }
        public DateTime dateStart { get; set; }
        public TimeSpan timeStart { get; set; }
        public string AddInformation { get; set; } = null!;
        public Location? location { get; set; }
    }
}