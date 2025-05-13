using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class Box
    {
        public long idMatch { get; }
        public int round { get; set; }
        public BoxWinner boxWinner { get; set; } = null!;
        public TimeSpan time { get; set; }
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 { get; set; } = null!;
        public List<MinusValue> playerMinusValue {get;set;} = null!;
        public List<PlayerFouls> playerFouls { get; set; } = null!;
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
    public class BoxWinner
    {
        public string FullNamePlayer { get; set; } = null!;
        public string loginWinner { get; set; } = null!;
        public MethodWin win { get; set; }
    }
    public class MinusValue
    {
        public string FullNamePlayer { get; set; } = null!;
        public int value { get; set; }
    }
}
