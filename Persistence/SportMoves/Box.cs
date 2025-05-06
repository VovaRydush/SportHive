using System.ComponentModel;
using MongoDB.Bson;

namespace DB.SportHive.MongoDb
{
    public class Box
    {
        public long idMatch { get;}
        public int round { get; set; }
        public TimeSpan time { get; set; }
        public string FullNamePlayer1 { get; set; } = null!;
        public BoxFouls? foulsPlayer1 { get; set; }
        public int? MinusValue1 { get; set; }
        public MethodWin? winPlayer1 { get; set; }
        public string FullNamePlayer2 { get; set; } = null!;
        public BoxFouls? foulsPlayer2 { get; set; }
        public int? MinusValue2 { get; set; }
        public MethodWin? winPlayer2 { get; set; }

    }
    public enum BoxFouls
    {
        [Description("LowBlow")]
        LowBlow, //удар нижче пояса
        [Description("LateHit")]
        LateHit, //удар після команди "стоп"
        [Description("RabbitPunch")]
        RabbitPunch, //удар у потилицю
        [Description("Holding")]
        Holding,
        [Description("Pushing")]
        Pushing,
        [Description("Elbow")]
        Elbow, //удар ліктем
        [Description("Headbutt")]
        Headbutt, //удар головою
    }
    public enum MethodWin
    {
        [Description("Knockout")]
        Knockout,
        [Description("Disqualification")]
        Disqualification,
        [Description("Pass")]
        Pass,
        [Description("TechnicalKnockout")]
        TechnicalKnockout,
        [Description("UnanimousDecision")]
        UnanimousDecision,
        [Description("SplitDecision")]
        SplitDecision
    }
}