namespace DB.SportHive.MongoDb
{
    public class TableTennis
    {
        public long idMatch { get; }
        public TypeTenis type { get; set; }
        public int SetCount { get; set; }
        public List<string> FullNamePlayer { get; set; } = null!;
        public TimeOut? timeOut { get; set; }
        public Card? card { get; set; }
        public TypeFoul foul { get; set; }
        public TimeSpan CreatedAt { get; set; }

    }
    public enum TypeFoul
    {
        FaultyServe,
        NetTouch,
        MissedBall,
        BallBounceTwice,
        WrongOrder,
        Obstructing,
        UnforcedError,
        ForcedError,
        DoubleFault,
        FootFault,
        Out,
        TimeViolation,
        CodeViolation,
        Carry,
        Disqualification

    }
    public enum TypeTenis
    {
        TableTennis,
        Tenis,
        Badminton
    }

}