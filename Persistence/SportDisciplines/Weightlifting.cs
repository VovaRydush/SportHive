namespace DB.SportHive.MongoDb
{
    public class WeightliftingMatch
    {
        public long IdMatch { get; }
        public List<Weightlifting> liftingsAthlete {get;set;} = null!;
    }
}