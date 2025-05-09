using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class Swimming
    {
        public long idMatch { get;}
        public bool Isdisqualification { get; set; }
        public string FullNamePlayer { get; set; } = null!;
        public TimeSpan time { get; set; }
        public SwimmingStyle style { get; set; }
        public Foul Foul { get; set; }

    }
    public enum SwimmingStyle
    {
        [Description("Butterfly")]
        Butterfly,
        [Description("Breaststroke")]
        Breaststroke,
        [Description("Rabbit")]
        Rabbit,
        [Description("Backstroke")]
        Backstroke,
        [Description("Comprehensive")]
        Comprehensive
    }
}
