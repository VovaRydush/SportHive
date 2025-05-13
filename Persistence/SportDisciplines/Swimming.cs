namespace DB.SportHive.MongoDb
{
    public class Swimming
    {
        public long idMatch { get;}
        public List<AthleteSwimming> athleteSwimming{get;set;} = null!;
    }
}