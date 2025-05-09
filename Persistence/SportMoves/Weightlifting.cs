using System.ComponentModel;

namespace DB.SportHive.MongoDb
{
    public class Weightlifting
    {
        public long IdMatch { get; }
        public WeightDesipline desipline { get; set; }
        public string WeightCategory { get; set; } = null!;
        public string FullNamePlayer { get; set; } = null!;
        public int countYes { get; set; }
        public int countNo { get; set; }
        public float Weight { get; set; }
        public int TryCount { get; set; }
        public bool done { get; set; }
        public Foul foul { get; set; }
    }
    public enum WeightDesipline
    {
        [Description("Snatch")]
        Snatch, //ривок
        [Description("CleanJerk")]
        CleanJerk, //поштовх
        [Description("Squat")]
        Squat, // присідання
        [Description("BenchPress")]
        BenchPress, //жим лежачи
        [Description("Deadlift")]
        Deadlift, // станова тяга
    }
}
