using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class AthleteSwimming
    {
        public long idMatch{ get; set; }
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer { get; set; } = null!;
        public TimeSpan time{ get; set; }
        public float AvgSpeed { get; set; }
        public int DistanceMeters { get; set; }
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
