namespace DB.SportHive.MongoDb
{
    public class RowingRace
    {
        public long idMatch { get; set; }
        public string AthleteOrTeam { get; set; } = null!;
        public int DistanceMeters { get; set; }
        public TimeSpan ResultTime { get; set; }
        public int Place { get; set; }
        public float? AverageSpeedKmh { get; set; }
        public bool Finished { get; set; } = true;
    }
}
