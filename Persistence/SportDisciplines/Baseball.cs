namespace DB.SportHive.MongoDb
{
    public class Baseball
    {
        public long IdMatch{get;set;}
        public int Tour{get;set;}
        public List<BaseballEvent> baseballEvents{get;set;} = null!;
        public List<PlayerFouls> playerFouls {get;set;} = null!;
    }
}