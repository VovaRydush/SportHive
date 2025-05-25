namespace DB.SportHive.MongoDb
{
    public class PlayerFouls
    {
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer{get;set;} = null!;
        public TimeSpan timeFoul {get;set;}
        public Foul foul { get; set; } = new();
        public Card? card { get; set; } = new();
    }
}