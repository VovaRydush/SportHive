namespace DB.SportHive.MongoDb
{
    public class PlayerFouls
    {
        public string FullNamePlayer { get; set; } = null!;
        public string loginPlayer{get;set;} = null!;
        public List<Foul> fouls { get; set; } = null!;
        public List<Card>? cards { get; set; } = null!;
    }
}