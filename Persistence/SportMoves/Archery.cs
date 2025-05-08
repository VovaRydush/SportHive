using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using MongoDB.Bson;
using StackExchange.Redis;

namespace DB.SportHive.MongoDb
{
    public class Archery
    {
        public long idMatch { get; set; }
        public string CompetitionType { get; set; } = null!;
        public string FullNamePlayer { get; set; } = null!;
        public string BowType { get; set; } = null!;
        public float Distance { get; set; }
        public ArcheryFoul foul { get; set; }
    }
    public enum ArcheryFoul
    {
        Overtime,
        WrongTarget,
        CrossingLineEarly,
        TooManyArrows
    }
}