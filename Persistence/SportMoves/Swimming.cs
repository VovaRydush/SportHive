using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class AthleteSwimmingStyle
    {
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer{get;set;} = null!;
        public SwimmingStyle style { get; set; }
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
