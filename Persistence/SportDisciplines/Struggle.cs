namespace DB.SportHive.MongoDb
{
    public class Struggle
    {
        public string typeResult { get; set; } = null!;
        public long idMatch { get;}
        public int round { get; set; }
        public TimeSpan time { get; set; }
        public List<PlayerFouls> player1Fouls {get;set;} = null!;
        public List<int>? MinusValue1 { get; set; } // в бородьбі за фоли можуть бали давати супернику
        public Result? winPlayer1 { get; set; }
        public List<PlayerFouls> player2Fouls {get;set;} = null!;
        public List<int>? MinusValue2 { get; set; } // в бородьбі за фоли можуть бали давати супернику
        public Result? winPlayer2 { get; set; }
    }
    public enum Result
    {
        Fall, // Туше 
        TechnicalSuperiority, // є вже 10 балів
        Points,
        Injury, // травма
        Rejection, // відмова
        Disqualification
    }
}
