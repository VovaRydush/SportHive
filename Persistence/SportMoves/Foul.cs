using System.ComponentModel;
using MongoDB.Bson;

namespace DB.SportHive.MongoDb
{
    public enum Card
    {
        [Description("Red")]
        Red,
        [Description("Yellow")]
        Yellow,
        [Description("FreeKick")]
        FreeKick,
        [Description("Removal")]
        Removal,
        [Description("Disqualification")]
        Disqualification,
        [Description("Warning")]
        Warning,
        [Description("Minifine")]
        Minifine,
        [Description("BigFine")]
        BigFine,
        [Description("PenaltyKick")]
        PenaltyKick
    }

    public enum Foul
    {
        // командні види спорту
        [Description("Personal")]
        Personal,           // Контактний фол
        [Description("Technical")]
        Technical,          // Технічний фол
        [Description("Handball")]
        Handball,           // Фол рукою (футбол)
        [Description("Blocking")]
        Blocking,           // Перешкоджання
        [Description("Shooting")]
        Shooting,           // Фол при кидку (баскетбол)
        [Description("Tripping")]
        Tripping,           // Підніжка
        // Вся важка атлетика
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
        Stepping, // Переставив ноги під час підйому

        // плавання 
        [Description("FalseStart")]
        FalseStart,
        [Description("BadTechnique")]
        BadTechnique,
        [Description("ViolationTurn")]
        ViolationTurn,
        [Description("Immersion")]
        Immersion,
        [Description("BatonViolation")]
        BatonViolation,

        // бородьба
        [Description("IllegalMove")]
        IllegalMove,
        [Description("FingerGrasp")]
        FingerGrasp,
        [Description("RoughPlay")]
        RoughPlay, // Удар, штовхання, ривок
        [Description("Passivity")]
        Passivity,
        [Description("HoldingGear")]
        HoldingGear,
        [Description("Avoiding")]
        Avoiding, // Вихід зі зони без боротьби
        [Description("Forfeit")]
        Forfeit,

        // перегони
        [Description("GateTouch")]
        GateTouch,
        [Description("GateMissed")]
        GateMissed,
        [Description("WrongGateDirection")]
        WrongGateDirection,
        // є також фальш старт

        // велікі
        [Description("UnsportingBehavior")]
        UnsportingBehavior,
        [Description("DraftingViolation")]
        DraftingViolation,
        [Description("LineChangeInSprint")]
        LineChangeInSprint,
        [Description("Littering")]
        Littering,
        [Description("IllegalAssistance")]
        IllegalAssistance,

        // теніс, бадмінтон
        [Description("FaultyServe")]
        FaultyServe,
        [Description("NetTouch")]
        NetTouch,
        [Description("MissedBall")]
        MissedBall,
        [Description("BallBounceTwice")]
        BallBounceTwice,
        [Description("WrongOrder")]
        WrongOrder,
        [Description("Obstructing")]
        Obstructing,
        [Description("UnforcedError")]
        UnforcedError,
        [Description("ForcedError")]
        ForcedError,
        [Description("DoubleFault")]
        DoubleFault,
        [Description("FootFault")]
        FootFault,
        [Description("Out")]
        Out,
        [Description("TimeViolation")]
        TimeViolation,
        [Description("CodeViolation")]
        CodeViolation,
        [Description("Carry")]
        Carry,

        // шахмати 
        [Description("TimeForfeit")]
        TimeForfeit,
        [Description("DeviceViolation")]
        DeviceViolation,
        [Description("CheatingSuspected")]
        CheatingSuspected,

        // шашки 
        [Description("MissedCapture")]
        MissedCapture,
        [Description("TouchViolation")]
        TouchViolation,

        // бокс

        [Description("LowBlow")]
        LowBlow, //удар нижче пояса
        [Description("LateHit")]
        LateHit, //удар після команди "стоп"
        [Description("RabbitPunch")]
        RabbitPunch, //удар у потилицю
        [Description("Holding")]
        Holding,
        [Description("Pushing")]
        Pushing,
        [Description("Elbow")]
        Elbow, //удар ліктем
        [Description("Headbutt")]
        Headbutt, //удар головою

        // стрільба з лука
        [Description("Overtime")]
        Overtime,
        [Description("WrongTarget")]
        WrongTarget,
        [Description("CrossingLineEarly")]
        CrossingLineEarly,
        [Description("TooManyArrows")]
        TooManyArrows
    }


}