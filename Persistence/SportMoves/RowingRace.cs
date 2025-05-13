namespace DB.SportHive.MongoDb
{
    public class RowingRace
    {
        public long IdRace { get; set; }
        public string AthleteOrTeam { get; set; } = null!;
        public int DistanceMeters { get; set; }
        public TimeSpan ResultTime { get; set; }
        public int Place { get; set; }
        public float? AverageSpeedKmh { get; set; }
        public List<Foul> Fouls { get; set; } = new();
        public bool Finished { get; set; } = true;
    }
}
