using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class PlayMoves 
    {
        public long IdMatch{ get; set; }
        public string FullNamePlayer{get;set;} = null!;
        public string timeMove {get;set;} = null!;
    }
    public enum TypeMove
    {
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
        DropGoal
    }
}
