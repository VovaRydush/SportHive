using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class MoveTwoPlayer 
    {
        public long IdMatch { get; set; }
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer { get; set; } = null!;
        public TypeMovePlayer typeMove{get;set;}
        public TimeSpan time { get; set; }
    }
    public enum TypeMovePlayer
    {
        [Description("Interception")]
        Interception,
        [Description("BlockShot")]
        BlockShot,
        [Description("Replacement")]
        Replacement
    }
}
