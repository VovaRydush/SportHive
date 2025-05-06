using System.ComponentModel;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using MongoDB.Bson;

namespace DB.SportHive.MongoDb
{
    public class Swimming
    {
        public long idMatch { get;}
        public bool Isdisqualification { get; set; }
        public string FullNamePlayer { get; set; } = null!;
        public TimeSpan time { get; set; }
        public SwimmingStyle style { get; set; }
        public SwimmingFoul foul { get; set; }

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
    public enum SwimmingFoul
    {
        [Description("FalseStart")]
        FalseStart,
        [Description("BadTechnique")]
        BadTechnique,
        [Description("ViolationTurn")]
        ViolationTurn,
        [Description("Immersion")]
        Immersion,
        [Description("BatonViolation")]
        BatonViolation
    }
}