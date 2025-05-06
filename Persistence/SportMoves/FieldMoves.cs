using System.ComponentModel;
using MongoDB.Bson;

namespace DB.SportHive.MongoDb
{
    public class FieldMoves
    {
        public long IdMatch { get;}
        public string FullNamePlayer { get; set; } = null!;
        FieldMove fieldMove { get; set; }
    }
    public enum FieldMove
    {
        [Description("AngleShot")]
        AngleShot,
        [Description("Freethrow")]
        Freethrow,
        [Description("Discarding")]
        Discarding
    }
}