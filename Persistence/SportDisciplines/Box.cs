using System.ComponentModel;
using DB.SportHive.Domain;
using SportHive.Services.Interfaces;
using SportHive.Implementations;
namespace DB.SportHive.MongoDb
{
    public class Box : MatchEvents
    {
        public Box(TwoPlayerInfo info)
        {
            idMatch = info.idMatch;
            FullNamePlayer1 = info.FullNamePlayer1;
            FullNamePlayer2 = info.FullNamePlayer2;
        }
        public int round { get; set; }
        public BoxWinner boxWinner { get; set; } = null!;
        public TimeSpan time { get; set; }
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 { get; set; } = null!;
        public List<MinusValue> playerMinusValue {get;set;} = null!;
        public List<PlayerFouls> playerFouls { get; set; } = null!;
    }

    internal interface ICompetitionSystem
    {
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
