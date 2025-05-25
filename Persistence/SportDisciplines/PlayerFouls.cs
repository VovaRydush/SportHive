namespace DB.SportHive.MongoDb
{
    public class PlayerFouls
    {
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer{get;set;} = null!;
        public List<TimeSpan> timeFouls {get;set;} = new();
        public List<Foul> fouls { get; set; } = new();
        public List<Card>? cards { get; set; } = new();
    }
}