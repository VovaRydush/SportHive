using System.ComponentModel;
using DB.SportHive.Domain;

namespace DB.SportHive.MongoDb
{
    public class Struggle : MatchEvents
    {
        public Struggle(){}
        public Struggle(TwoPlayerInfo info)
        {
            idEvent = info.idEvent;
            idMatch = info.idMatch;
            tour = info.tour;
            FullNamePlayer1 = info.FullNamePlayer1;
            FullNamePlayer2 = info.FullNamePlayer2;
        }
        public int tour { get; set; }
        public string typeResult { get; set; } = null!;
        public WinStruggleResult winner { get; set; } = null!;
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 {get;set;} = null!;
        public int round { get; set; }
        public List<RoundPoints> points { get; set; } = new();
        public List<PlayerFouls> player1Fouls { get; set; } = new();
        public List<PlayerFouls> player2Fouls {get;set;} = new();
    }
    public class WinStruggleResult 
    {
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer { get; set; } = null!;
        public int? round { get; set; }
        public int countPoints { get; set; }
        public Result win { get; set; }
    }
    public class RoundPoints
    {
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer { get; set; } = null!;
        public int round { get; set; }
        public int countPoints { get; set; }
    }
    public enum Result
    {
        [Description("Fall")]
        Fall, // Туше 
        [Description("TechnicalSuperiority")]
        TechnicalSuperiority, // є вже 10 балів
        [Description("Points")]
        Points,
        [Description("Injury")]
        Injury, // травма
        [Description("Rejection")]
        Rejection, // відмова
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
