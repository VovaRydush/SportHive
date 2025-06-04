using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class PlayMoves 
    {
        public long IdMatch{ get; set; }
        public string FullNamePlayer{get;set;} = null!;
        public string loginPlayer { get; set; } = null!;
        public TypeMove typeMove { get; set; }
        public TimeSpan timeMove { get; set; }
    }
    public enum TypeMove
    {
        [Description("Tries")]
        Tries,
        [Description("StealBasketball")]
        StealBasketball,
        [Description("Offside")]
        Offside,
        [Description("Goal")]
        Goal,
        [Description("AutoGoal")]
        AutoGoal,
        [Description("OffensiveRebound")]
        OffensiveRebound,
        [Description("DefensiveRebound")]
        DefensiveRebound,
        [Description("LossBoll")]
        LossBoll,
        [Description("Save")]
        Save,
        [Description("FieldGoal")]
        FieldGoal,
        [Description("Safety")]
        Safety,
        [Description("DropGoal")]
        DropGoal,
        [Description("Tackles")]
        Tackles,
        [Description("Bullseyes")]
        Bullseyes
    }
}
