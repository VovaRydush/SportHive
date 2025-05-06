using MongoDB.Bson;

namespace DB.SportHive.MongoDb
{
    public class DistanceRunning
    {
        public long idMatch { get; set; }
        public string FullNamePlayer { get; set; } = null!;
        public TimeSpan timeFinish { get; set; }
        public int Round { get; set; }
    }
}