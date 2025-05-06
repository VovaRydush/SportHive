using System.ComponentModel;
using MongoDB.Bson;

namespace DB.SportHive.MongoDb
{
    public class Athletics
    {
        public long IdMatch { get; set; }
        public TypeDesipline typeDesipline { get; set; }
        public float distance { get; set; }
        public int Try { get; set; }
    }
    public enum TypeDesipline
    {
        [Description("LongJump")]
        LongJump,
        [Description("HighJump")]
        HighJump,
        [Description("CorePushing")]
        CorePushing,
        [Description("DiscusThrowing")]
        DiscusThrowing,
    }
}