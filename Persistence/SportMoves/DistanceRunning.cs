namespace DB.SportHive.MongoDb
{
    public class DistanceRunning : MatchEvents
    {
        public long idMatch { get; set; }
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer { get; set; } = null!;
        public TimeSpan timeFinish { get; set; }
    }
}
