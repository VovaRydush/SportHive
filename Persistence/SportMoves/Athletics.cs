using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class AthleticsMoves
    {
        public string FullNamePlayer { get; set; } = null!;
        public string login { get; set; } = null!;
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
