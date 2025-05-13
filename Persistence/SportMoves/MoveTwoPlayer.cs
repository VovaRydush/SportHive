using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class MoveTwoPlayer
    {
        public long IdMatch { get; }
        public string FullNamePlayer1 { get; set; } = null!;
        public string FullNamePlayer2 { get; set; } = null!;
        public TypeMovePlayer typeMove{get;set;}
        public string time { get; set; } = null!;
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
