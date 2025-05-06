using System.ComponentModel;
using MongoDB.Bson;

namespace DB.SportHive.MongoDb
{
    public class Foul
    {
        public long IdMatch { get; }
        public string FullNamePlayer { get; set; } = null!;
        public string timeFoal { get; set; } = null!;
        public Card card { get; set; }
        public FoulType foulType{get;set;}

    }
    public enum Card
    {
        [Description("Red")]
        Red,
        [Description("Yellow")]
        Yellow,
        [Description("FreeKick")]
        FreeKick,
        [Description("Removal")]
        Removal,
        [Description("Disqualification")]
        Disqualification,
        [Description("Warning")]
        Warning,
        [Description("Minifine")]
        Minifine,
        [Description("BigFine")]
        BigFine,
        [Description("PenaltyKick")]
        PenaltyKick
    }

    public enum FoulType
    {
        [Description("Personal")]
        Personal,           // Контактний фол
        [Description("Technical")]
        Technical,          // Технічний фол
        [Description("Unsportsmanlike")]
        Unsportsmanlike,    // Неспортивна поведінка
        [Description("Handball")]
        Handball,           // Фол рукою (футбол)
        [Description("Blocking")]
        Blocking,           // Перешкоджання
        [Description("Shooting")]
        Shooting,           // Фол при кидку (баскетбол)
        [Description("Tripping")]
        Tripping,           // Підніжка
    }


}