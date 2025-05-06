using System.ComponentModel;
using MongoDB.Bson;

namespace DB.SportHive.MongoDb
{
    public class AttacksMoves
    {
        public long IdMatch{get;set;}
        public string FullNamePlayer{get;set;} = null!;
        public string time {get;set;} = null!;
        public bool realization {get;set;}
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