using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class Touchdown
    {
        public long IdMatch { get; }
        public string FullNamePlayer { get; set; } = null!;
        public TypeTouchdown typeTouchdown { get; set; }
        public int yards { get; set; }
        public bool realization { get; set; }
        public string time { get; set; } = null!;
    }
    public enum TypeTouchdown
    {
        [Description("pass")]
        pass,
        [Description("rush")]
        rush
    }
}