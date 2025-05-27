namespace DB.SportHive.MongoDb
{
    public class CyclingRace
    {
        public long IdMatch { get; set; }
        public string AthleteName { get; set; } = null!;
        public string loginPlayer { get; set; } = null!;
        public float DistanceKm { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int Position { get; set; }
        public float AvgSpeed { get; set; }
        public float? MaxSpeed { get; set; }
        public bool DidNotFinish { get; set; }
    }
}

