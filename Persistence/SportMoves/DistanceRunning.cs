namespace DB.SportHive.MongoDb
{
    public class DistanceRunning
    {
        public string FullNamePlayer { get; set; } = null!;
        public TimeSpan timeFinish { get; set; }
        public int Round { get; set; }
    }
}
