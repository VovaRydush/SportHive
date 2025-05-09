using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class AttacksMoves
    {
        public long IdMatch { get; }
        public string FullNamePlayer { get; set; } = null!;
        public string time { get; set; } = null!;
        public TypeMoves move { get; set; }
        public bool realization { get; set; }
    }
    public enum TypeMoves
    {
        [Description("Serving")]
        Serving,
        [Description("Attack")]
        Attack,
        [Description("Block")]
        Block,
        [Description("ShotOnGoal")]
        ShotOnGoal,
        [Description("Pass")]
        Pass,
        [Description("Penalty")]
        Penalty,
        [Description("Conversion")]
        Conversion
    }
}
