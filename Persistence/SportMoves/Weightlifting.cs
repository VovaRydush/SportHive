using System.ComponentModel;
using System.Globalization;

namespace DB.SportHive.MongoDb
{
    public class Weightlifting
    {
        public long IdMatch{get;set;}
        public WeightDesipline desipline{get;set;}
        public string WeightCategory {get;set;} = null!;
        public string FullNamePlayer {get;set;} = null!;
        public int countYes{get;set;}
        public int countNo{get;set;}
        public float Weight {get;set;}
        public int TryCount {get;set;}
        public ResultMove result{get;set;}

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
    public enum ResultMove
    {
        [Description("Disqualification")]
        Disqualification,
        [Description("BadLift")]
        BadLift, //Недопідйом
        [Description("EarlyDrop")]
        EarlyDrop,
        [Description("PlatformExit")]
        PlatformExit,
        [Description("DoubleMovement")]
        DoubleMovement,
        [Description("CoachInterference")]
        CoachInterference,
        [Description("BombOut")]
        BombOut,
        [Description("Done")]
        Done,
        [Description("NoLockout")]
        NoLockout, // не зафіксовано вагу
        [Description("MissedСommand")]
        MissedСommand,
        [Description("UnevenMovement")]
        UnevenMovement,
        [Description("BarDropped")]
        BarDropped, //впав інвентар
        [Description("LiftingOff")]
        LiftingOff, // відрив
        [Description("ShallowSquat")]
        ShallowSquat,// недостатня глибина присідання
        [Description("EarlyMovement")]
        EarlyMovement, // Рух назад вгору до команди
        [Description("UnevenBar")]
        UnevenBar, // Плечі не вирівняні
        [Description("Bounce")]
        Bounce, // не торкнувся грудей
        [Description("RoundedBack")]
        RoundedBack, //не стабільне положення
        [Description("Stepping")]
        Stepping // Переставив ноги під час підйому
    }
}