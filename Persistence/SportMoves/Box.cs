using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class Box
    {
        public long idMatch { get;}
        public int round { get; set; }
        public TimeSpan time { get; set; }
        public string FullNamePlayer1 { get; set; } = null!;
        public Foul? foulsPlayer1 { get; set; }
        public int? MinusValue1 { get; set; }
        public MethodWin? winPlayer1 { get; set; }
        public string FullNamePlayer2 { get; set; } = null!;
        public Foul? foulsPlayer2 { get; set; }
        public int? MinusValue2 { get; set; }
        public MethodWin? winPlayer2 { get; set; }

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
