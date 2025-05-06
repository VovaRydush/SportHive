namespace DB.SportHive.MongoDb
{
    public class TimeOut
    {
        public long IdMatch { get; }
        public string NameTeam { get; set; } = null!;
        public string TimeStartTimeOut { get; set; } = null!;
        public string TimeEndTimeOut { get; set; } = null!;
    }
}